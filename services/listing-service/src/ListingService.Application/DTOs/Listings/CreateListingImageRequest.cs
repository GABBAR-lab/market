namespace ListingService.Application.DTOs.Listings;

public record CreateListingImageRequest(
    string Url,
    string? ThumbnailUrl,
    string? AltText,
    int SortOrder,
    bool IsPrimary);
