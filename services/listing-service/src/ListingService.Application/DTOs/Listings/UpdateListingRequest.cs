namespace ListingService.Application.DTOs.Listings;

public record UpdateListingRequest(
    Guid CategoryId,
    string Title,
    string Slug,
    string Description,
    decimal? Price,
    string Currency,
    string PriceType,
    string Condition,
    Guid? LocationId,
    string? City,
    string? District,
    string? Province,
    string Country,
    string? ContactPhone,
    string? ContactEmail,
    bool ShowPhone,
    bool ShowEmail,
    DateTime? ExpiresAt);
