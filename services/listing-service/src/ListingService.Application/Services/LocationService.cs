using ListingService.Application.Common;
using ListingService.Application.DTOs.Locations;
using ListingService.Application.Interfaces;
using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<Result<LocationResponse>> CreateAsync(CreateLocationRequest request)
    {
        if (!Enum.TryParse<LocationType>(request.Type, ignoreCase: true, out var locationType))
        {
            return Result<LocationResponse>.Failure("Invalid location type.");
        }

        if (await _locationRepository.SlugExistsAsync(request.Slug, request.ParentLocationId))
        {
            return Result<LocationResponse>.Failure("Location slug already exists.");
        }

        if (request.ParentLocationId.HasValue)
        {
            var parent = await _locationRepository.GetByIdAsync(request.ParentLocationId.Value);
            if (parent is null)
            {
                return Result<LocationResponse>.Failure("Parent location not found.");
            }
        }

        var location = Location.Create(request.Name, request.Slug, locationType, request.ParentLocationId, request.SortOrder);
        await _locationRepository.AddAsync(location);

        return Result<LocationResponse>.Success(MapToResponse(location));
    }

    public async Task<Result<LocationResponse>> GetByIdAsync(Guid id)
    {
        var location = await _locationRepository.GetByIdAsync(id, includeChildren: true);
        if (location is null)
        {
            return Result<LocationResponse>.Failure("Location not found.");
        }

        return Result<LocationResponse>.Success(MapToResponse(location, includeChildren: true));
    }

    public async Task<Result<LocationResponse>> GetBySlugAsync(string slug)
    {
        var location = await _locationRepository.GetBySlugAsync(slug);
        if (location is null)
        {
            return Result<LocationResponse>.Failure("Location not found.");
        }

        return Result<LocationResponse>.Success(MapToResponse(location));
    }

    public async Task<Result<IReadOnlyList<LocationResponse>>> GetAllAsync()
    {
        var locations = await _locationRepository.GetAllAsync();
        var roots = locations.Where(l => l.ParentLocationId is null).ToList();
        return Result<IReadOnlyList<LocationResponse>>.Success(roots.Select(l => MapToResponse(l)).ToList());
    }

    public async Task<Result<IReadOnlyList<LocationResponse>>> GetByTypeAsync(string type)
    {
        if (!Enum.TryParse<LocationType>(type, ignoreCase: true, out var locationType))
        {
            return Result<IReadOnlyList<LocationResponse>>.Failure("Invalid location type.");
        }

        var locations = await _locationRepository.GetByTypeAsync(locationType);
        return Result<IReadOnlyList<LocationResponse>>.Success(locations.Select(l => MapToResponse(l)).ToList());
    }

    public async Task<Result<IReadOnlyList<LocationResponse>>> GetChildrenAsync(Guid parentLocationId)
    {
        var children = await _locationRepository.GetChildrenAsync(parentLocationId);
        return Result<IReadOnlyList<LocationResponse>>.Success(children.Select(l => MapToResponse(l)).ToList());
    }

    public async Task<Result<LocationResponse>> UpdateAsync(Guid id, UpdateLocationRequest request)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location is null)
        {
            return Result<LocationResponse>.Failure("Location not found.");
        }

        if (await _locationRepository.SlugExistsAsync(request.Slug, location.ParentLocationId, id))
        {
            return Result<LocationResponse>.Failure("Location slug already exists.");
        }

        location.Update(request.Name, request.Slug, request.SortOrder, request.IsActive);
        await _locationRepository.UpdateAsync(location);

        return Result<LocationResponse>.Success(MapToResponse(location));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location is null)
        {
            return Result<bool>.Failure("Location not found.");
        }

        location.Update(location.Name, location.Slug, location.SortOrder, false);
        await _locationRepository.UpdateAsync(location);

        return Result<bool>.Success(true);
    }

    private static LocationResponse MapToResponse(Location location, bool includeChildren = false)
    {
        IReadOnlyList<LocationResponse>? children = null;
        if (includeChildren && location.Children.Count > 0)
        {
            children = location.Children.Select(c => MapToResponse(c)).ToList();
        }

        return new LocationResponse(
            location.Id,
            location.Name,
            location.Slug,
            location.Type.ToString(),
            location.ParentLocationId,
            location.SortOrder,
            location.IsActive,
            children,
            location.CreatedAt,
            location.UpdatedAt);
    }
}
