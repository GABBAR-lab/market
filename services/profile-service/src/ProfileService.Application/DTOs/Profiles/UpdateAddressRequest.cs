namespace ProfileService.Application.DTOs.Profiles;

public record UpdateAddressRequest(
    string Label,
    string StreetLine1,
    string? StreetLine2,
    string City,
    string State,
    string Country,
    string PostalCode,
    bool IsDefault = false);
