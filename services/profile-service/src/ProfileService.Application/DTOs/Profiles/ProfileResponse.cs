namespace ProfileService.Application.DTOs.Profiles;

public record ProfileResponse(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string? Bio,
    string? AvatarUrl,
    DateTime? DateOfBirth,
    string? Gender,
    string? PhoneNumber,
    string? Website,
    string Language,
    string Currency,
    string Timezone,
    bool EmailNotifications,
    bool SmsNotifications,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<AddressResponse> Addresses);
