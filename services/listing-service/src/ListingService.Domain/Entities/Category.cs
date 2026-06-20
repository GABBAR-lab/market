using ListingService.Domain.Common;

namespace ListingService.Domain.Entities;

public class Category : AuditableEntity
{
    private readonly List<Category> _subCategories = [];
    private readonly List<CategoryAttribute> _attributes = [];

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? IconUrl { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Category? ParentCategory { get; private set; }
    public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();
    public IReadOnlyCollection<CategoryAttribute> Attributes => _attributes.AsReadOnly();

    public static Category Create(
        string name,
        string slug,
        string? description = null,
        string? iconUrl = null,
        Guid? parentCategoryId = null,
        int sortOrder = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return new Category
        {
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            Description = description?.Trim(),
            IconUrl = iconUrl?.Trim(),
            ParentCategoryId = parentCategoryId,
            SortOrder = sortOrder,
            IsActive = true
        };
    }

    public void Update(
        string name,
        string slug,
        string? description,
        string? iconUrl,
        int sortOrder,
        bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Description = description?.Trim();
        IconUrl = iconUrl?.Trim();
        SortOrder = sortOrder;
        IsActive = isActive;
        MarkAsUpdated();
    }

    public CategoryAttribute AddAttribute(
        string name,
        string displayName,
        Enums.AttributeFieldType fieldType,
        string? options,
        bool isRequired,
        bool isFilterable,
        int sortOrder)
    {
        var attribute = CategoryAttribute.Create(
            Id,
            name,
            displayName,
            fieldType,
            options,
            isRequired,
            isFilterable,
            sortOrder);

        _attributes.Add(attribute);
        MarkAsUpdated();
        return attribute;
    }

    internal static Category Restore(
        Guid id,
        string name,
        string slug,
        string? description,
        string? iconUrl,
        Guid? parentCategoryId,
        int sortOrder,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Slug = slug,
            Description = description,
            IconUrl = iconUrl,
            ParentCategoryId = parentCategoryId,
            SortOrder = sortOrder,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    internal void LoadSubCategories(IEnumerable<Category> subCategories)
    {
        _subCategories.Clear();
        _subCategories.AddRange(subCategories);
    }

    internal void LoadAttributes(IEnumerable<CategoryAttribute> attributes)
    {
        _attributes.Clear();
        _attributes.AddRange(attributes);
    }
}
