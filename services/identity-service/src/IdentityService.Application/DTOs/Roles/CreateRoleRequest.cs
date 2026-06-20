namespace IdentityService.Application.DTOs.Roles;

public record CreateRoleRequest(string Name, string? Description = null);
