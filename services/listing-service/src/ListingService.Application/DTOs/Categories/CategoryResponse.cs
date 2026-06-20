namespace ListingService.Application.DTOs.Categories;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    Guid? ParentCategoryId,
    int SortOrder,
    bool IsActive,
    int ListingCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
