using ListingService.Domain.Common;
using ListingService.Domain.Enums;

namespace ListingService.Domain.Entities;

public class Location : AuditableEntity
{
    private readonly List<Location> _children = [];

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public LocationType Type { get; private set; }
    public Guid? ParentLocationId { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Location? ParentLocation { get; private set; }
    public IReadOnlyCollection<Location> Children => _children.AsReadOnly();

    public static Location Create(
        string name,
        string slug,
        LocationType type,
        Guid? parentLocationId = null,
        int sortOrder = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return new Location
        {
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            Type = type,
            ParentLocationId = parentLocationId,
            SortOrder = sortOrder,
            IsActive = true
        };
    }

    public void Update(string name, string slug, int sortOrder, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        SortOrder = sortOrder;
        IsActive = isActive;
        MarkAsUpdated();
    }

    internal static Location Restore(
        Guid id,
        string name,
        string slug,
        LocationType type,
        Guid? parentLocationId,
        int sortOrder,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Location
        {
            Id = id,
            Name = name,
            Slug = slug,
            Type = type,
            ParentLocationId = parentLocationId,
            SortOrder = sortOrder,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    internal void LoadChildren(IEnumerable<Location> children)
    {
        _children.Clear();
        _children.AddRange(children);
    }
}
