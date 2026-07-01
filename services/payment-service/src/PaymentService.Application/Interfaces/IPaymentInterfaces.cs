using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Interfaces;

public interface IPaymentRepository
{
    Task<PaymentTransaction> AddAsync(PaymentTransaction payment);
    Task<IReadOnlyList<PaymentTransaction>> GetBySellerAsync(Guid sellerId);
}

public interface IListingServiceClient
{
    Task<ListingCategoryDto?> GetCategoryAsync(Guid categoryId);
    Task<ListingForPaymentDto?> GetListingAsync(Guid listingId);
    Task<bool> ActivateListingAfterPaymentAsync(Guid listingId, bool requireApproval);
    Task<IReadOnlyList<CategoryPricingResponse>> GetCategoryPricingAsync();
    Task<CategoryPricingResponse?> UpdateCategoryPricingAsync(Guid categoryId, UpdateCategoryPricingRequest request);
}

public interface IPaymentAppService
{
    Task<Common.Result<PaymentCalculationResponse>> CalculateAsync(CalculatePaymentRequest request);
    Task<Common.Result<PaymentResponse>> CompletePaymentAsync(Guid sellerId, CompletePaymentRequest request);
    Task<Common.Result<IReadOnlyList<CategoryPricingResponse>>> GetAllCategoryPricingAsync();
    Task<Common.Result<CategoryPricingResponse>> UpdateCategoryPricingAsync(Guid categoryId, UpdateCategoryPricingRequest request);
}
