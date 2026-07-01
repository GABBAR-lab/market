namespace ListingService.Application.DTOs.Listings;

public record ListingDetailResponse(
    Guid Id,
    Guid SellerId,
    Guid CategoryId,
    string CategoryName,
    string Title,
    string Slug,
    string Description,
    decimal? Price,
    string Currency,
    string PriceType,
    string Condition,
    string Status,
    Guid? LocationId,
    string? LocationName,
    string? City,
    string? District,
    string? Province,
    string Country,
    double? Latitude,
    double? Longitude,
    string? ContactPhone,
    string? ContactEmail,
    bool ShowPhone,
    bool ShowEmail,
    int ViewCount,
    bool IsFeatured,
    DateTime? FeaturedUntil,
    DateTime? PublishedAt,
    DateTime? ExpiresAt,
    IReadOnlyList<ListingImageResponse> Images,
    IReadOnlyList<ListingAttributeValueResponse> Attributes,
    string? ListingPurpose,
    int AdDurationDays,
    decimal? PaymentAmount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record ListingImageResponse(
    Guid Id,
    string Url,
    string? ThumbnailUrl,
    string? AltText,
    int SortOrder,
    bool IsPrimary);

public record ListingAttributeValueResponse(
    Guid Id,
    Guid CategoryAttributeId,
    string AttributeName,
    string DisplayName,
    string Value);
