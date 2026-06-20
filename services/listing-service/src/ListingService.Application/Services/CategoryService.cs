using ListingService.Application.Common;
using ListingService.Application.DTOs.Categories;
using ListingService.Application.Interfaces;
using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        if (await _categoryRepository.SlugExistsAsync(request.Slug))
        {
            return Result<CategoryResponse>.Failure("Category slug already exists.");
        }

        if (request.ParentCategoryId.HasValue)
        {
            var parent = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value);
            if (parent is null)
            {
                return Result<CategoryResponse>.Failure("Parent category not found.");
            }
        }

        var category = Category.Create(
            request.Name,
            request.Slug,
            request.Description,
            request.IconUrl,
            request.ParentCategoryId,
            request.SortOrder);

        await _categoryRepository.AddAsync(category);

        var count = await _categoryRepository.GetListingCountAsync(category.Id);
        return Result<CategoryResponse>.Success(MapToResponse(category, count));
    }

    public async Task<Result<CategoryDetailResponse>> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id, includeAttributes: true);
        if (category is null)
        {
            return Result<CategoryDetailResponse>.Failure("Category not found.");
        }

        return Result<CategoryDetailResponse>.Success(await MapToDetailResponseAsync(category));
    }

    public async Task<Result<CategoryDetailResponse>> GetBySlugAsync(string slug)
    {
        var category = await _categoryRepository.GetBySlugAsync(slug, includeAttributes: true);
        if (category is null)
        {
            return Result<CategoryDetailResponse>.Failure("Category not found.");
        }

        return Result<CategoryDetailResponse>.Success(await MapToDetailResponseAsync(category));
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var rootCategories = categories.Where(c => c.ParentCategoryId is null).ToList();
        var responses = new List<CategoryResponse>();

        foreach (var category in rootCategories)
        {
            var count = await _categoryRepository.GetListingCountAsync(category.Id);
            responses.Add(MapToResponse(category, count));
        }

        return Result<IReadOnlyList<CategoryResponse>>.Success(responses);
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetSubCategoriesAsync(Guid parentCategoryId)
    {
        var subCategories = await _categoryRepository.GetSubCategoriesAsync(parentCategoryId);
        var responses = new List<CategoryResponse>();

        foreach (var category in subCategories)
        {
            var count = await _categoryRepository.GetListingCountAsync(category.Id);
            responses.Add(MapToResponse(category, count));
        }

        return Result<IReadOnlyList<CategoryResponse>>.Success(responses);
    }

    public async Task<Result<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return Result<CategoryResponse>.Failure("Category not found.");
        }

        if (await _categoryRepository.SlugExistsAsync(request.Slug, id))
        {
            return Result<CategoryResponse>.Failure("Category slug already exists.");
        }

        category.Update(request.Name, request.Slug, request.Description, request.IconUrl, request.SortOrder, request.IsActive);
        await _categoryRepository.UpdateAsync(category);

        var count = await _categoryRepository.GetListingCountAsync(category.Id);
        return Result<CategoryResponse>.Success(MapToResponse(category, count));
    }

    public async Task<Result<CategoryAttributeResponse>> AddAttributeAsync(Guid categoryId, CreateCategoryAttributeRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, includeAttributes: true);
        if (category is null)
        {
            return Result<CategoryAttributeResponse>.Failure("Category not found.");
        }

        if (!Enum.TryParse<AttributeFieldType>(request.FieldType, ignoreCase: true, out var fieldType))
        {
            return Result<CategoryAttributeResponse>.Failure("Invalid field type.");
        }

        var attribute = category.AddAttribute(
            request.Name,
            request.DisplayName,
            fieldType,
            request.Options,
            request.IsRequired,
            request.IsFilterable,
            request.SortOrder);

        await _categoryRepository.UpdateAsync(category);

        return Result<CategoryAttributeResponse>.Success(MapAttribute(attribute));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return Result<bool>.Failure("Category not found.");
        }

        var count = await _categoryRepository.GetListingCountAsync(id);
        if (count > 0)
        {
            return Result<bool>.Failure("Cannot delete category with active listings.");
        }

        category.Update(category.Name, category.Slug, category.Description, category.IconUrl, category.SortOrder, false);
        await _categoryRepository.UpdateAsync(category);

        return Result<bool>.Success(true);
    }

    private async Task<CategoryDetailResponse> MapToDetailResponseAsync(Category category)
    {
        var count = await _categoryRepository.GetListingCountAsync(category.Id);
        var subCategories = await _categoryRepository.GetSubCategoriesAsync(category.Id);
        var subResponses = new List<CategoryResponse>();

        foreach (var sub in subCategories)
        {
            var subCount = await _categoryRepository.GetListingCountAsync(sub.Id);
            subResponses.Add(MapToResponse(sub, subCount));
        }

        return new CategoryDetailResponse(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IconUrl,
            category.ParentCategoryId,
            category.SortOrder,
            category.IsActive,
            count,
            subResponses,
            category.Attributes.Select(MapAttribute).ToList(),
            category.CreatedAt,
            category.UpdatedAt);
    }

    private static CategoryResponse MapToResponse(Category category, int listingCount) => new(
        category.Id,
        category.Name,
        category.Slug,
        category.Description,
        category.IconUrl,
        category.ParentCategoryId,
        category.SortOrder,
        category.IsActive,
        listingCount,
        category.CreatedAt,
        category.UpdatedAt);

    private static CategoryAttributeResponse MapAttribute(CategoryAttribute attribute) => new(
        attribute.Id,
        attribute.CategoryId,
        attribute.Name,
        attribute.DisplayName,
        attribute.FieldType.ToString(),
        attribute.Options,
        attribute.IsRequired,
        attribute.IsFilterable,
        attribute.SortOrder);
}
