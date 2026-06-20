using ListingService.Application.Common;
using ListingService.Application.DTOs.Categories;

namespace ListingService.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request);
    Task<Result<CategoryDetailResponse>> GetByIdAsync(Guid id);
    Task<Result<CategoryDetailResponse>> GetBySlugAsync(string slug);
    Task<Result<IReadOnlyList<CategoryResponse>>> GetAllAsync();
    Task<Result<IReadOnlyList<CategoryResponse>>> GetSubCategoriesAsync(Guid parentCategoryId);
    Task<Result<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request);
    Task<Result<CategoryAttributeResponse>> AddAttributeAsync(Guid categoryId, CreateCategoryAttributeRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}
