namespace IdentityService.Application.DTOs.Auth;

public record AuthResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
