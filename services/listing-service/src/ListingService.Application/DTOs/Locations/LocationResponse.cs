namespace ListingService.Application.DTOs.Locations;

public record LocationResponse(
    Guid Id,
    string Name,
    string Slug,
    string Type,
    Guid? ParentLocationId,
    int SortOrder,
    bool IsActive,
    IReadOnlyList<LocationResponse>? Children,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
