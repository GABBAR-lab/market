namespace IdentityService.Application.DTOs.Users;

public record UserResponse(
    Guid Id,
    string Email,
    bool EmailVerified,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Status,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    IReadOnlyList<string> Roles);
