using ListingService.Application.Common;
using ListingService.Application.DTOs.Payments;
using ListingService.Application.Interfaces;
using ListingService.Domain.Enums;

namespace ListingService.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAppSettingsRepository _appSettingsRepository;

    public PaymentService(
        ICategoryRepository categoryRepository,
        IListingRepository listingRepository,
        IPaymentRepository paymentRepository,
        IAppSettingsRepository appSettingsRepository)
    {
        _categoryRepository = categoryRepository;
        _listingRepository = listingRepository;
        _paymentRepository = paymentRepository;
        _appSettingsRepository = appSettingsRepository;
    }

    public async Task<Result<PaymentCalculationResponse>> CalculateAsync(CalculatePaymentRequest request)
    {
        if (request.DurationDays < 1 || request.DurationDays > 365)
        {
            return Result<PaymentCalculationResponse>.Failure("Duration must be between 1 and 365 days.");
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category is null || !category.IsActive)
        {
            return Result<PaymentCalculationResponse>.Failure("Category not found.");
        }

        var perDay = GetPerDayPrice(category, request.ListingPurpose);
        var total = perDay * request.DurationDays;

        return Result<PaymentCalculationResponse>.Success(new PaymentCalculationResponse(
            category.Id,
            category.Name,
            request.ListingPurpose,
            request.DurationDays,
            perDay,
            total,
            "LKR"));
    }

    public async Task<Result<PaymentResponse>> CompletePaymentAsync(Guid sellerId, CompletePaymentRequest request)
    {
        var listing = await _listingRepository.GetByIdAsync(request.ListingId, includeDetails: false);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<PaymentResponse>.Failure("Listing not found.");
        }

        if (listing.SellerId != sellerId)
        {
            return Result<PaymentResponse>.Failure("Not authorized to pay for this listing.");
        }

        if (listing.Status != ListingStatus.PendingPayment)
        {
            return Result<PaymentResponse>.Failure("Listing is not awaiting payment.");
        }

        if (!ValidateCard(request, out var cardError))
        {
            return Result<PaymentResponse>.Failure(cardError!);
        }

        var category = await _categoryRepository.GetByIdAsync(listing.CategoryId);
        if (category is null)
        {
            return Result<PaymentResponse>.Failure("Category not found.");
        }

        var perDay = GetPerDayPrice(category, listing.ListingPurpose ?? "Sale");
        var amount = listing.PaymentAmount ?? perDay * listing.AdDurationDays;
        var transactionRef = $"LL-{Guid.NewGuid():N}"[..20].ToUpperInvariant();
        var lastFour = request.CardNumber[^4..];

        var paymentId = await _paymentRepository.CreatePaymentAsync(
            listing.Id,
            sellerId,
            amount,
            listing.AdDurationDays,
            perDay,
            "Card",
            transactionRef,
            lastFour);

        var requireApproval = await _appSettingsRepository.GetBoolAsync("RequireAdminApproval", false);
        listing.ActivateAfterPayment(requireApproval);
        await _listingRepository.UpdateAsync(listing);
        await _paymentRepository.MarkCompletedAsync(paymentId);

        return Result<PaymentResponse>.Success(new PaymentResponse(
            paymentId,
            listing.Id,
            amount,
            "LKR",
            "Completed",
            transactionRef,
            listing.Status.ToString(),
            DateTime.UtcNow));
    }

    public async Task<Result<IReadOnlyList<CategoryPricingResponse>>> GetAllCategoryPricingAsync()
    {
        var items = await _categoryRepository.GetRootCategoryPricingAsync();
        return Result<IReadOnlyList<CategoryPricingResponse>>.Success(items);
    }

    public async Task<Result<CategoryPricingResponse>> UpdateCategoryPricingAsync(
        Guid categoryId,
        UpdateCategoryPricingRequest request)
    {
        if (request.PerDayPriceSale < 0 || request.PerDayPriceBuy < 0 || request.PerDayPriceRent < 0)
        {
            return Result<CategoryPricingResponse>.Failure("Prices cannot be negative.");
        }

        var updated = await _categoryRepository.UpdatePricingAsync(
            categoryId,
            request.PerDayPriceSale,
            request.PerDayPriceBuy,
            request.PerDayPriceRent);

        if (updated is null)
        {
            return Result<CategoryPricingResponse>.Failure("Category not found.");
        }

        return Result<CategoryPricingResponse>.Success(updated);
    }

    private static decimal GetPerDayPrice(Domain.Entities.Category category, string purpose) =>
        purpose.Trim().ToLowerInvariant() switch
        {
            "buy" => category.PerDayPriceBuy,
            "rent" => category.PerDayPriceRent,
            _ => category.PerDayPriceSale
        };

    private static bool ValidateCard(CompletePaymentRequest request, out string? error)
    {
        var digits = new string(request.CardNumber.Where(char.IsDigit).ToArray());
        if (digits.Length < 13 || digits.Length > 19)
        {
            error = "Invalid card number.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.CardHolderName))
        {
            error = "Card holder name is required.";
            return false;
        }

        if (!int.TryParse(request.ExpiryMonth, out var month) || month is < 1 or > 12)
        {
            error = "Invalid expiry month.";
            return false;
        }

        if (!int.TryParse(request.ExpiryYear, out var year) || year < DateTime.UtcNow.Year)
        {
            error = "Invalid expiry year.";
            return false;
        }

        if (request.Cvv.Length is < 3 or > 4 || !request.Cvv.All(char.IsDigit))
        {
            error = "Invalid CVV.";
            return false;
        }

        error = null;
        return true;
    }
}
