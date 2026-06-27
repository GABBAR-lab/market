namespace ListingService.Application.DTOs.Categories;

public record CategoryTreeNodeResponse(
    Guid Id,
    string Name,
    string Slug,
    string? IconUrl,
    int ListingCount,
    string? SearchTerm,
    IReadOnlyList<CategoryTreeNodeResponse> SubCategories);
