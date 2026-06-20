using ListingService.Application.Common;
using ListingService.Application.DTOs.Locations;

namespace ListingService.Application.Interfaces;

public interface ILocationService
{
    Task<Result<LocationResponse>> CreateAsync(CreateLocationRequest request);
    Task<Result<LocationResponse>> GetByIdAsync(Guid id);
    Task<Result<LocationResponse>> GetBySlugAsync(string slug);
    Task<Result<IReadOnlyList<LocationResponse>>> GetAllAsync();
    Task<Result<IReadOnlyList<LocationResponse>>> GetByTypeAsync(string type);
    Task<Result<IReadOnlyList<LocationResponse>>> GetChildrenAsync(Guid parentLocationId);
    Task<Result<LocationResponse>> UpdateAsync(Guid id, UpdateLocationRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}
