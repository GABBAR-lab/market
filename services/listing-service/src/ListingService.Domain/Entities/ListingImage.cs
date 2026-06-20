using ListingService.Domain.Common;

namespace ListingService.Domain.Entities;

public class ListingImage : AuditableEntity
{
    public Guid ListingId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; private set; }
    public string? AltText { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    public Listing? Listing { get; private set; }

    public static ListingImage Create(
        Guid listingId,
        string url,
        string? thumbnailUrl,
        string? altText,
        int sortOrder,
        bool isPrimary)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        if (listingId == Guid.Empty)
        {
            throw new ArgumentException("ListingId is required.", nameof(listingId));
        }

        return new ListingImage
        {
            ListingId = listingId,
            Url = url.Trim(),
            ThumbnailUrl = thumbnailUrl?.Trim(),
            AltText = altText?.Trim(),
            SortOrder = sortOrder,
            IsPrimary = isPrimary
        };
    }

    public void Update(string url, string? thumbnailUrl, string? altText, int sortOrder, bool isPrimary)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        Url = url.Trim();
        ThumbnailUrl = thumbnailUrl?.Trim();
        AltText = altText?.Trim();
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
        MarkAsUpdated();
    }

    public void ClearPrimary()
    {
        IsPrimary = false;
        MarkAsUpdated();
    }

    internal static ListingImage Restore(
        Guid id,
        Guid listingId,
        string url,
        string? thumbnailUrl,
        string? altText,
        int sortOrder,
        bool isPrimary,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new ListingImage
        {
            Id = id,
            ListingId = listingId,
            Url = url,
            ThumbnailUrl = thumbnailUrl,
            AltText = altText,
            SortOrder = sortOrder,
            IsPrimary = isPrimary,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
}
