using ListingService.Domain.Common;
using ListingService.Domain.Enums;

namespace ListingService.Domain.Entities;

public class Listing : AuditableEntity
{
    private readonly List<ListingImage> _images = [];
    private readonly List<ListingAttributeValue> _attributeValues = [];

    public Guid SellerId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal? Price { get; private set; }
    public string Currency { get; private set; } = "LKR";
    public PriceType PriceType { get; private set; } = PriceType.Fixed;
    public ListingCondition Condition { get; private set; } = ListingCondition.NotApplicable;
    public ListingStatus Status { get; private set; } = ListingStatus.Draft;

    public Guid? LocationId { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public string? Province { get; private set; }
    public string Country { get; private set; } = "Sri Lanka";
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }
    public bool ShowPhone { get; private set; } = true;
    public bool ShowEmail { get; private set; }

    public int ViewCount { get; private set; }
    public bool IsFeatured { get; private set; }
    public DateTime? FeaturedUntil { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public Category? Category { get; private set; }
    public Location? Location { get; private set; }
    public IReadOnlyCollection<ListingImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<ListingAttributeValue> AttributeValues => _attributeValues.AsReadOnly();

    public static Listing Create(
        Guid sellerId,
        Guid categoryId,
        string title,
        string slug,
        string description,
        decimal? price,
        string currency,
        PriceType priceType,
        ListingCondition condition,
        Guid? locationId,
        string? city,
        string? district,
        string? province,
        string country,
        string? contactPhone,
        string? contactEmail,
        bool showPhone,
        bool showEmail,
        DateTime? expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        if (sellerId == Guid.Empty)
        {
            throw new ArgumentException("SellerId is required.", nameof(sellerId));
        }

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("CategoryId is required.", nameof(categoryId));
        }

        return new Listing
        {
            SellerId = sellerId,
            CategoryId = categoryId,
            Title = title.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            Description = description.Trim(),
            Price = price,
            Currency = currency.Trim(),
            PriceType = priceType,
            Condition = condition,
            LocationId = locationId,
            City = city?.Trim(),
            District = district?.Trim(),
            Province = province?.Trim(),
            Country = country.Trim(),
            ContactPhone = contactPhone?.Trim(),
            ContactEmail = contactEmail?.Trim(),
            ShowPhone = showPhone,
            ShowEmail = showEmail,
            Status = ListingStatus.Draft,
            ExpiresAt = expiresAt
        };
    }

    public void Update(
        Guid categoryId,
        string title,
        string slug,
        string description,
        decimal? price,
        string currency,
        PriceType priceType,
        ListingCondition condition,
        Guid? locationId,
        string? city,
        string? district,
        string? province,
        string country,
        string? contactPhone,
        string? contactEmail,
        bool showPhone,
        bool showEmail,
        DateTime? expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        CategoryId = categoryId;
        Title = title.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Description = description.Trim();
        Price = price;
        Currency = currency.Trim();
        PriceType = priceType;
        Condition = condition;
        LocationId = locationId;
        City = city?.Trim();
        District = district?.Trim();
        Province = province?.Trim();
        Country = country.Trim();
        ContactPhone = contactPhone?.Trim();
        ContactEmail = contactEmail?.Trim();
        ShowPhone = showPhone;
        ShowEmail = showEmail;
        ExpiresAt = expiresAt;
        MarkAsUpdated();
    }

    public ListingImage AddImage(string url, string? thumbnailUrl, string? altText, int sortOrder, bool isPrimary)
    {
        if (isPrimary)
        {
            foreach (var existingImage in _images)
            {
                existingImage.ClearPrimary();
            }
        }

        var newImage = ListingImage.Create(Id, url, thumbnailUrl, altText, sortOrder, isPrimary);
        _images.Add(newImage);
        MarkAsUpdated();
        return newImage;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new InvalidOperationException("Image not found.");

        _images.Remove(image);
        MarkAsUpdated();
    }

    public void SetAttributeValue(Guid categoryAttributeId, string value)
    {
        var existing = _attributeValues.FirstOrDefault(a => a.CategoryAttributeId == categoryAttributeId);

        if (existing is not null)
        {
            existing.Update(value);
        }
        else
        {
            _attributeValues.Add(ListingAttributeValue.Create(Id, categoryAttributeId, value));
        }

        MarkAsUpdated();
    }

    public void Publish()
    {
        Status = ListingStatus.Active;
        PublishedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsSold()
    {
        Status = ListingStatus.Sold;
        MarkAsUpdated();
    }

    public void Expire()
    {
        Status = ListingStatus.Expired;
        MarkAsUpdated();
    }

    public void Reject()
    {
        Status = ListingStatus.Rejected;
        MarkAsUpdated();
    }

    public void SubmitForReview()
    {
        Status = ListingStatus.PendingReview;
        MarkAsUpdated();
    }

    public void Feature(DateTime featuredUntil)
    {
        IsFeatured = true;
        FeaturedUntil = featuredUntil;
        MarkAsUpdated();
    }

    public void RemoveFeatured()
    {
        IsFeatured = false;
        FeaturedUntil = null;
        MarkAsUpdated();
    }

    public void IncrementViewCount()
    {
        ViewCount++;
        MarkAsUpdated();
    }

    public void SoftDelete()
    {
        Status = ListingStatus.Deleted;
        MarkAsUpdated();
    }

    internal static Listing Restore(
        Guid id,
        Guid sellerId,
        Guid categoryId,
        string title,
        string slug,
        string description,
        decimal? price,
        string currency,
        PriceType priceType,
        ListingCondition condition,
        ListingStatus status,
        Guid? locationId,
        string? city,
        string? district,
        string? province,
        string country,
        double? latitude,
        double? longitude,
        string? contactPhone,
        string? contactEmail,
        bool showPhone,
        bool showEmail,
        int viewCount,
        bool isFeatured,
        DateTime? featuredUntil,
        DateTime? publishedAt,
        DateTime? expiresAt,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Listing
        {
            Id = id,
            SellerId = sellerId,
            CategoryId = categoryId,
            Title = title,
            Slug = slug,
            Description = description,
            Price = price,
            Currency = currency,
            PriceType = priceType,
            Condition = condition,
            Status = status,
            LocationId = locationId,
            City = city,
            District = district,
            Province = province,
            Country = country,
            Latitude = latitude,
            Longitude = longitude,
            ContactPhone = contactPhone,
            ContactEmail = contactEmail,
            ShowPhone = showPhone,
            ShowEmail = showEmail,
            ViewCount = viewCount,
            IsFeatured = isFeatured,
            FeaturedUntil = featuredUntil,
            PublishedAt = publishedAt,
            ExpiresAt = expiresAt,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    internal void LoadImages(IEnumerable<ListingImage> images)
    {
        _images.Clear();
        _images.AddRange(images);
    }

    internal void LoadAttributeValues(IEnumerable<ListingAttributeValue> attributeValues)
    {
        _attributeValues.Clear();
        _attributeValues.AddRange(attributeValues);
    }

    internal void SetCategory(Category? category) => Category = category;

    internal void SetLocation(Location? location) => Location = location;
}
