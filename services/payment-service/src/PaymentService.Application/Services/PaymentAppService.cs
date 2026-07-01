using MarketPlace.Shared.Contracts.Events;
using MarketPlace.Shared.Messaging;
using PaymentService.Application.Common;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Services;

public class PaymentAppService : IPaymentAppService
{
    private readonly IListingServiceClient _listingClient;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventPublisher _eventPublisher;

    public PaymentAppService(
        IListingServiceClient listingClient,
        IPaymentRepository paymentRepository,
        IEventPublisher eventPublisher)
    {
        _listingClient = listingClient;
        _paymentRepository = paymentRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<PaymentCalculationResponse>> CalculateAsync(CalculatePaymentRequest request)
    {
        if (request.DurationDays is < 1 or > 365)
        {
            return Result<PaymentCalculationResponse>.Failure("Duration must be between 1 and 365 days.");
        }

        var category = await _listingClient.GetCategoryAsync(request.CategoryId);
        if (category is null || !category.IsActive)
        {
            return Result<PaymentCalculationResponse>.Failure("Category not found.");
        }

        var perDay = GetPerDayPrice(category, request.ListingPurpose);
        return Result<PaymentCalculationResponse>.Success(new PaymentCalculationResponse(
            category.Id, category.Name, request.ListingPurpose, request.DurationDays, perDay, perDay * request.DurationDays, "LKR"));
    }

    public async Task<Result<PaymentResponse>> CompletePaymentAsync(Guid sellerId, CompletePaymentRequest request)
    {
        var listing = await _listingClient.GetListingAsync(request.ListingId);
        if (listing is null)
        {
            return Result<PaymentResponse>.Failure("Listing not found.");
        }

        if (listing.SellerId != sellerId)
        {
            return Result<PaymentResponse>.Failure("Not authorized to pay for this listing.");
        }

        if (listing.Status != "PendingPayment")
        {
            return Result<PaymentResponse>.Failure("Listing is not awaiting payment.");
        }

        if (!ValidateCard(request, out var cardError))
        {
            return Result<PaymentResponse>.Failure(cardError!);
        }

        var category = await _listingClient.GetCategoryAsync(listing.CategoryId);
        if (category is null)
        {
            return Result<PaymentResponse>.Failure("Category not found.");
        }

        var perDay = GetPerDayPrice(category, listing.ListingPurpose ?? "Sale");
        var amount = listing.PaymentAmount ?? perDay * listing.AdDurationDays;
        var transactionRef = $"LL-{Guid.NewGuid():N}"[..20].ToUpperInvariant();

        var payment = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            ListingId = listing.Id,
            SellerId = sellerId,
            Amount = amount,
            DurationDays = listing.AdDurationDays,
            PerDayPrice = perDay,
            TransactionRef = transactionRef,
            CardLastFour = request.CardNumber[^4..],
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(payment);

        var requireApproval = false;
        var activated = await _listingClient.ActivateListingAfterPaymentAsync(listing.Id, requireApproval);
        if (!activated)
        {
            return Result<PaymentResponse>.Failure("Failed to activate listing after payment.");
        }

        var listingStatus = requireApproval ? "PendingReview" : "Active";

        await _eventPublisher.PublishAsync(EventTypes.PaymentCompleted, new PaymentCompletedPayload(
            payment.Id, listing.Id, sellerId, amount, "LKR", transactionRef, listingStatus));

        await _eventPublisher.PublishAsync(EventTypes.NotificationSend, new NotificationSendPayload(
            sellerId, "Payment successful", $"Your ad payment of Rs {amount:N0} was completed.", "InApp", "Listing", listing.Id));

        return Result<PaymentResponse>.Success(new PaymentResponse(
            payment.Id, listing.Id, amount, "LKR", "Completed", transactionRef, listingStatus, payment.PaidAt));
    }

    public async Task<Result<IReadOnlyList<CategoryPricingResponse>>> GetAllCategoryPricingAsync()
    {
        var items = await _listingClient.GetCategoryPricingAsync();
        return Result<IReadOnlyList<CategoryPricingResponse>>.Success(items);
    }

    public async Task<Result<CategoryPricingResponse>> UpdateCategoryPricingAsync(Guid categoryId, UpdateCategoryPricingRequest request)
    {
        if (request.PerDayPriceSale < 0 || request.PerDayPriceBuy < 0 || request.PerDayPriceRent < 0)
        {
            return Result<CategoryPricingResponse>.Failure("Prices cannot be negative.");
        }

        var updated = await _listingClient.UpdateCategoryPricingAsync(categoryId, request);
        return updated is null
            ? Result<CategoryPricingResponse>.Failure("Category not found.")
            : Result<CategoryPricingResponse>.Success(updated);
    }

    private static decimal GetPerDayPrice(ListingCategoryDto category, string purpose) =>
        purpose.Trim().ToLowerInvariant() switch
        {
            "buy" => category.PerDayPriceBuy,
            "rent" => category.PerDayPriceRent,
            _ => category.PerDayPriceSale
        };

    private static bool ValidateCard(CompletePaymentRequest request, out string? error)
    {
        var digits = new string(request.CardNumber.Where(char.IsDigit).ToArray());
        if (digits.Length is < 13 or > 19) { error = "Invalid card number."; return false; }
        if (string.IsNullOrWhiteSpace(request.CardHolderName)) { error = "Card holder name is required."; return false; }
        if (!int.TryParse(request.ExpiryMonth, out var month) || month is < 1 or > 12) { error = "Invalid expiry month."; return false; }
        if (!int.TryParse(request.ExpiryYear, out var year) || year < DateTime.UtcNow.Year) { error = "Invalid expiry year."; return false; }
        if (request.Cvv.Length is < 3 or > 4 || !request.Cvv.All(char.IsDigit)) { error = "Invalid CVV."; return false; }
        error = null;
        return true;
    }
}
