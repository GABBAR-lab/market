using IdentityService.Application.Common;
using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;

namespace IdentityService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return Result<AuthResponse>.Failure("Email is already registered.");
        }

        var user = User.Create(
            request.Email,
            _passwordHasher.Hash(request.Password),
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        var buyerRole = await _roleRepository.GetByNameAsync("Buyer");
        var roleNames = new List<string>();

        if (buyerRole is not null)
        {
            user.AssignRole(buyerRole);
            roleNames.Add(buyerRole.Name);
        }

        var accessToken = _tokenService.GenerateAccessToken(user, roleNames);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshExpiry = _tokenService.GetRefreshTokenExpiry();
        user.AddRefreshToken(refreshToken, refreshExpiry);

        await _userRepository.AddAsync(user);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id,
            user.Email,
            accessToken,
            refreshToken,
            _tokenService.GetAccessTokenExpiry()));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        if (user.Status == UserStatus.Suspended)
        {
            return Result<AuthResponse>.Failure("Account is suspended.");
        }

        if (user.Status == UserStatus.Deleted)
        {
            return Result<AuthResponse>.Failure("Account has been deleted.");
        }

        user.RecordLogin();

        return await BuildAuthResponse(user);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user is null || !user.RefreshTokens.Any(t => t.Token == request.RefreshToken && t.IsActive))
        {
            return Result<AuthResponse>.Failure("Invalid or expired refresh token.");
        }

        var oldToken = user.RefreshTokens.First(t => t.Token == request.RefreshToken);
        oldToken.Revoke();

        return await BuildAuthResponse(user);
    }

    public async Task<Result<bool>> LogoutAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user is null)
        {
            return Result<bool>.Failure("Refresh token not found.");
        }

        var token = user.RefreshTokens.First(t => t.Token == refreshToken);
        token.Revoke();
        await _userRepository.UpdateAsync(user);

        return Result<bool>.Success(true);
    }

    private async Task<Result<AuthResponse>> BuildAuthResponse(User user)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshExpiry = _tokenService.GetRefreshTokenExpiry();

        user.AddRefreshToken(refreshToken, refreshExpiry);
        await _userRepository.UpdateAsync(user);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id,
            user.Email,
            accessToken,
            refreshToken,
            _tokenService.GetAccessTokenExpiry()));
    }
}
