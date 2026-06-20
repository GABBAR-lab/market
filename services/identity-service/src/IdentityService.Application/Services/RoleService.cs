using IdentityService.Application.Common;
using IdentityService.Application.DTOs.Roles;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<RoleResponse>> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role is null)
        {
            return Result<RoleResponse>.Failure("Role not found.");
        }

        return Result<RoleResponse>.Success(MapToResponse(role));
    }

    public async Task<Result<IReadOnlyList<RoleResponse>>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Result<IReadOnlyList<RoleResponse>>.Success(roles.Select(MapToResponse).ToList());
    }

    public async Task<Result<RoleResponse>> CreateAsync(CreateRoleRequest request)
    {
        var existing = await _roleRepository.GetByNameAsync(request.Name);
        if (existing is not null)
        {
            return Result<RoleResponse>.Failure("Role with this name already exists.");
        }

        var role = Role.Create(request.Name, request.Description);
        await _roleRepository.AddAsync(role);

        return Result<RoleResponse>.Success(MapToResponse(role));
    }

    public async Task<Result<RoleResponse>> UpdateAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role is null)
        {
            return Result<RoleResponse>.Failure("Role not found.");
        }

        role.Update(request.Name, request.Description);
        await _roleRepository.UpdateAsync(role);

        return Result<RoleResponse>.Success(MapToResponse(role));
    }

    private static RoleResponse MapToResponse(Role role) => new(
        role.Id,
        role.Name,
        role.Description,
        role.Type.ToString(),
        role.CreatedAt);
}
