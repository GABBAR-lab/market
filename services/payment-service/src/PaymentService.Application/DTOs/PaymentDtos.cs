namespace PaymentService.Application.DTOs;

public record CalculatePaymentRequest(Guid CategoryId, string ListingPurpose, int DurationDays);

public record PaymentCalculationResponse(
    Guid CategoryId, string CategoryName, string ListingPurpose, int DurationDays,
    decimal PerDayPrice, decimal TotalAmount, string Currency);

public record CompletePaymentRequest(
    Guid ListingId, string CardNumber, string CardHolderName,
    string ExpiryMonth, string ExpiryYear, string Cvv);

public record PaymentResponse(
    Guid PaymentId, Guid ListingId, decimal Amount, string Currency, string Status,
    string? TransactionRef, string ListingStatus, DateTime? PaidAt);

public record UpdateCategoryPricingRequest(decimal PerDayPriceSale, decimal PerDayPriceBuy, decimal PerDayPriceRent);

public record CategoryPricingResponse(
    Guid Id, string Name, string Slug, decimal PerDayPriceSale, decimal PerDayPriceBuy, decimal PerDayPriceRent);

public record ListingCategoryDto(Guid Id, string Name, decimal PerDayPriceSale, decimal PerDayPriceBuy, decimal PerDayPriceRent, bool IsActive);

public record ListingForPaymentDto(
    Guid Id, Guid SellerId, Guid CategoryId, string Status, string? ListingPurpose,
    int AdDurationDays, decimal? PaymentAmount);
