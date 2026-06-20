namespace ListingService.Application.DTOs.Locations;

public record CreateLocationRequest(
    string Name,
    string Slug,
    string Type,
    Guid? ParentLocationId,
    int SortOrder);
