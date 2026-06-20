namespace IdentityService.Application.DTOs.Users;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber = null);
