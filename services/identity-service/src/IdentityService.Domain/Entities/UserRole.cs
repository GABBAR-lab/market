using IdentityService.Domain.Common;

namespace IdentityService.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;

    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    public static UserRole Create(User user, Role role)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(role);

        return new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        };
    }

    /// <summary>Seed/migration helper — assign role without loading the full user graph.</summary>
    public static UserRole Assign(Guid userId, Guid roleId)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };
    }
}
