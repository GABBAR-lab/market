namespace ListingService.Application.DTOs.Categories;

public record CreateCategoryRequest(
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    Guid? ParentCategoryId,
    int SortOrder);
