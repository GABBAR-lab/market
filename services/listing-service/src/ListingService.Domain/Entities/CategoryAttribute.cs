using ListingService.Domain.Common;
using ListingService.Domain.Enums;

namespace ListingService.Domain.Entities;

public class CategoryAttribute : AuditableEntity
{
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public AttributeFieldType FieldType { get; private set; }
    public string? Options { get; private set; }
    public bool IsRequired { get; private set; }
    public bool IsFilterable { get; private set; }
    public int SortOrder { get; private set; }

    public Category Category { get; private set; } = null!;

    public static CategoryAttribute Create(
        Guid categoryId,
        string name,
        string displayName,
        AttributeFieldType fieldType,
        string? options,
        bool isRequired,
        bool isFilterable,
        int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("CategoryId is required.", nameof(categoryId));
        }

        return new CategoryAttribute
        {
            CategoryId = categoryId,
            Name = name.Trim().ToLowerInvariant(),
            DisplayName = displayName.Trim(),
            FieldType = fieldType,
            Options = options?.Trim(),
            IsRequired = isRequired,
            IsFilterable = isFilterable,
            SortOrder = sortOrder
        };
    }

    public void Update(
        string displayName,
        AttributeFieldType fieldType,
        string? options,
        bool isRequired,
        bool isFilterable,
        int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        DisplayName = displayName.Trim();
        FieldType = fieldType;
        Options = options?.Trim();
        IsRequired = isRequired;
        IsFilterable = isFilterable;
        SortOrder = sortOrder;
        MarkAsUpdated();
    }

    internal static CategoryAttribute Restore(
        Guid id,
        Guid categoryId,
        string name,
        string displayName,
        AttributeFieldType fieldType,
        string? options,
        bool isRequired,
        bool isFilterable,
        int sortOrder,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new CategoryAttribute
        {
            Id = id,
            CategoryId = categoryId,
            Name = name,
            DisplayName = displayName,
            FieldType = fieldType,
            Options = options,
            IsRequired = isRequired,
            IsFilterable = isFilterable,
            SortOrder = sortOrder,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
}
