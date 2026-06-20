using IdentityService.Application.Common;
using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Result<UserResponse>.Failure("User not found.");
        }

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    public async Task<Result<IReadOnlyList<UserResponse>>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return Result<IReadOnlyList<UserResponse>>.Success(users.Select(MapToResponse).ToList());
    }

    public async Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Result<UserResponse>.Failure("User not found.");
        }

        user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        await _userRepository.UpdateAsync(user);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Result<bool>.Failure("User not found.");
        }

        user.SoftDelete();
        await _userRepository.UpdateAsync(user);

        return Result<bool>.Success(true);
    }

    public async Task<Result<UserResponse>> VerifyEmailAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Result<UserResponse>.Failure("User not found.");
        }

        user.VerifyEmail();
        await _userRepository.UpdateAsync(user);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    public async Task<Result<UserResponse>> SuspendAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Result<UserResponse>.Failure("User not found.");
        }

        user.Suspend();
        await _userRepository.UpdateAsync(user);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    public async Task<Result<UserResponse>> ActivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return Result<UserResponse>.Failure("User not found.");
        }

        user.Activate();
        await _userRepository.UpdateAsync(user);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    public async Task<Result<UserResponse>> AssignRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Result<UserResponse>.Failure("User not found.");
        }

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role is null)
        {
            return Result<UserResponse>.Failure("Role not found.");
        }

        user.AssignRole(role);
        await _userRepository.UpdateAsync(user);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    public async Task<Result<UserResponse>> RemoveRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Result<UserResponse>.Failure("User not found.");
        }

        user.RemoveRole(roleId);
        await _userRepository.UpdateAsync(user);

        return Result<UserResponse>.Success(MapToResponse(user));
    }

    private static UserResponse MapToResponse(User user) => new(
        user.Id,
        user.Email,
        user.EmailVerified,
        user.FirstName,
        user.LastName,
        user.PhoneNumber,
        user.Status.ToString(),
        user.LastLoginAt,
        user.CreatedAt,
        user.UserRoles.Select(ur => ur.Role.Name).ToList());
}
