namespace ListingService.Application.DTOs.Locations;

public record UpdateLocationRequest(
    string Name,
    string Slug,
    int SortOrder,
    bool IsActive);
