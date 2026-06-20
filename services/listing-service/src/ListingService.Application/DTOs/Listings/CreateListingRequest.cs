namespace ListingService.Application.DTOs.Listings;

public record CreateListingRequest(
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
    DateTime? ExpiresAt,
    IReadOnlyList<CreateListingImageRequest>? Images,
    IReadOnlyList<SetListingAttributeRequest>? Attributes);

public record CreateListingImageRequest(
    string Url,
    string? ThumbnailUrl,
    string? AltText,
    int SortOrder,
    bool IsPrimary);

public record SetListingAttributeRequest(
    Guid CategoryAttributeId,
    string Value);
