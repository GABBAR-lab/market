using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Application.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, bool includeChildren = false);
    Task<Location?> GetBySlugAsync(string slug);
    Task<IReadOnlyList<Location>> GetAllAsync(bool activeOnly = true);
    Task<IReadOnlyList<Location>> GetByTypeAsync(LocationType type, bool activeOnly = true);
    Task<IReadOnlyList<Location>> GetChildrenAsync(Guid parentLocationId);
    Task AddAsync(Location location);
    Task UpdateAsync(Location location);
    Task<bool> SlugExistsAsync(string slug, Guid? parentLocationId, Guid? excludeId = null);
}
