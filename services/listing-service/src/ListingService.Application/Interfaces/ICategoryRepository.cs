using ListingService.Application.DTOs.Payments;
using ListingService.Domain.Entities;

namespace ListingService.Application.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, bool includeAttributes = false);
    Task<Category?> GetBySlugAsync(string slug, bool includeAttributes = false);
    Task<IReadOnlyList<Category>> GetAllAsync(bool activeOnly = true);
    Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentCategoryId);
    Task<IReadOnlyList<CategoryAttribute>> GetAttributesByCategoryIdAsync(Guid categoryId);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<int> GetListingCountAsync(Guid categoryId);
    Task<IReadOnlyList<CategoryPricingResponse>> GetRootCategoryPricingAsync();
    Task<CategoryPricingResponse?> UpdatePricingAsync(Guid categoryId, decimal sale, decimal buy, decimal rent);
}
