namespace ProfileService.Application.DTOs.Profiles;

public record CreateProfileRequest(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Bio = null,
    string? AvatarUrl = null,
    DateTime? DateOfBirth = null,
    string? Gender = null,
    string? PhoneNumber = null,
    string? Website = null,
    string Language = "en",
    string Currency = "USD",
    string Timezone = "UTC",
    bool EmailNotifications = true,
    bool SmsNotifications = false);
