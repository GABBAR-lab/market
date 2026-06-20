using IdentityService.Application.Common;
using IdentityService.Application.DTOs.Users;

namespace IdentityService.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserResponse>> GetByIdAsync(Guid id);
    Task<Result<IReadOnlyList<UserResponse>>> GetAllAsync();
    Task<Result<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<UserResponse>> VerifyEmailAsync(Guid id);
    Task<Result<UserResponse>> SuspendAsync(Guid id);
    Task<Result<UserResponse>> ActivateAsync(Guid id);
    Task<Result<UserResponse>> AssignRoleAsync(Guid userId, Guid roleId);
    Task<Result<UserResponse>> RemoveRoleAsync(Guid userId, Guid roleId);
}
