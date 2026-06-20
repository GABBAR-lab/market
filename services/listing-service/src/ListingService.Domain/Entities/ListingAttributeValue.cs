using ListingService.Domain.Common;

namespace ListingService.Domain.Entities;

public class ListingAttributeValue : AuditableEntity
{
    public Guid ListingId { get; private set; }
    public Guid CategoryAttributeId { get; private set; }
    public string Value { get; private set; } = string.Empty;

    public Listing? Listing { get; private set; }
    public CategoryAttribute? CategoryAttribute { get; private set; }

    public static ListingAttributeValue Create(Guid listingId, Guid categoryAttributeId, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (listingId == Guid.Empty)
        {
            throw new ArgumentException("ListingId is required.", nameof(listingId));
        }

        if (categoryAttributeId == Guid.Empty)
        {
            throw new ArgumentException("CategoryAttributeId is required.", nameof(categoryAttributeId));
        }

        return new ListingAttributeValue
        {
            ListingId = listingId,
            CategoryAttributeId = categoryAttributeId,
            Value = value.Trim()
        };
    }

    public void Update(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim();
        MarkAsUpdated();
    }

    internal static ListingAttributeValue Restore(
        Guid id,
        Guid listingId,
        Guid categoryAttributeId,
        string value,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new ListingAttributeValue
        {
            Id = id,
            ListingId = listingId,
            CategoryAttributeId = categoryAttributeId,
            Value = value,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    internal void SetCategoryAttribute(CategoryAttribute attribute) => CategoryAttribute = attribute;
}
