namespace ListingService.Application.DTOs.Categories;

public record CreateCategoryAttributeRequest(
    string Name,
    string DisplayName,
    string FieldType,
    string? Options,
    bool IsRequired,
    bool IsFilterable,
    int SortOrder);
