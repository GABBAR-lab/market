using ListingService.Application.Common;
using ListingService.Application.DTOs.Payments;

namespace ListingService.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentCalculationResponse>> CalculateAsync(CalculatePaymentRequest request);
    Task<Result<PaymentResponse>> CompletePaymentAsync(Guid sellerId, CompletePaymentRequest request);
    Task<Result<IReadOnlyList<CategoryPricingResponse>>> GetAllCategoryPricingAsync();
    Task<Result<CategoryPricingResponse>> UpdateCategoryPricingAsync(Guid categoryId, UpdateCategoryPricingRequest request);
}
