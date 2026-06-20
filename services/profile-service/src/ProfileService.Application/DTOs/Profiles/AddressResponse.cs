namespace ProfileService.Application.DTOs.Profiles;

public record AddressResponse(
    Guid Id,
    string Label,
    string StreetLine1,
    string? StreetLine2,
    string City,
    string State,
    string Country,
    string PostalCode,
    bool IsDefault,
    DateTime CreatedAt);
