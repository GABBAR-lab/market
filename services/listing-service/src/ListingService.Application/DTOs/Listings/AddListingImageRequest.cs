namespace ListingService.Application.DTOs.Listings;

public record AddListingImageRequest(
    string Url,
    string? ThumbnailUrl,
    string? AltText,
    int SortOrder,
    bool IsPrimary);
