using IdentityService.Application.Common;
using IdentityService.Application.DTOs.Roles;

namespace IdentityService.Application.Interfaces;

public interface IRoleService
{
    Task<Result<RoleResponse>> GetByIdAsync(Guid id);
    Task<Result<IReadOnlyList<RoleResponse>>> GetAllAsync();
    Task<Result<RoleResponse>> CreateAsync(CreateRoleRequest request);
    Task<Result<RoleResponse>> UpdateAsync(Guid id, UpdateRoleRequest request);
}
