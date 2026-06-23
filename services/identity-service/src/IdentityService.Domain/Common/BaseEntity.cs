namespace IdentityService.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    internal void OverrideId(Guid id) => Id = id;
}
