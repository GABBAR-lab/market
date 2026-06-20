namespace IdentityService.Application.DTOs.Roles;

public record RoleResponse(
    Guid Id,
    string Name,
    string? Description,
    string Type,
    DateTime CreatedAt);
