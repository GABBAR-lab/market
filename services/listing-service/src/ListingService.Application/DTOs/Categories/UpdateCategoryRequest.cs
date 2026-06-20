namespace ListingService.Application.DTOs.Categories;

public record UpdateCategoryRequest(
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    int SortOrder,
    bool IsActive);
