namespace IdentityService.Application.DTOs.Roles;

public record UpdateRoleRequest(string Name, string? Description = null);
