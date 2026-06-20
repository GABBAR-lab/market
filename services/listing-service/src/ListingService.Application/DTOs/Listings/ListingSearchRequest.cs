namespace ListingService.Application.DTOs.Listings;

public record ListingSearchRequest(
    string? SearchTerm,
    Guid? CategoryId,
    Guid? LocationId,
    string? City,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Status,
    bool? IsFeatured,
    Guid? SellerId,
    string SortBy = "newest",
    int Page = 1,
    int PageSize = 20);
