using IdentityService.Domain.Common;
using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public class Role : AuditableEntity
{
    private readonly List<UserRole> _userRoles = [];

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public RoleType Type { get; private set; } = RoleType.Custom;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    public static Role Create(string name, string? description = null, RoleType type = RoleType.Custom)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Role
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Type = type
        };
    }

    public static Role CreateSystemRole(string name, string? description = null)
    {
        return Create(name, description, RoleType.System);
    }

    public void Update(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
        Description = description?.Trim();
        MarkAsUpdated();
    }
}
