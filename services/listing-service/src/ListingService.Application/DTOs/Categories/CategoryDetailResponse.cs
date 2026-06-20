namespace ListingService.Application.DTOs.Categories;

public record CategoryDetailResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    Guid? ParentCategoryId,
    int SortOrder,
    bool IsActive,
    int ListingCount,
    IReadOnlyList<CategoryResponse> SubCategories,
    IReadOnlyList<CategoryAttributeResponse> Attributes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CategoryAttributeResponse(
    Guid Id,
    Guid CategoryId,
    string Name,
    string DisplayName,
    string FieldType,
    string? Options,
    bool IsRequired,
    bool IsFilterable,
    int SortOrder);
