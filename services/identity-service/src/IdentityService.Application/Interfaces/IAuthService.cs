using IdentityService.Application.Common;
using IdentityService.Application.DTOs.Auth;

namespace IdentityService.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result<bool>> LogoutAsync(string refreshToken);
}
