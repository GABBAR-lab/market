using System.Data;
using System.Text;
using Dapper;
using ListingService.Application.DTOs.Listings;
using ListingService.Application.Interfaces;
using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Infrastructure.Persistence.Repositories;

public class ListingRepository : IListingRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ListingRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Listing?> GetByIdAsync(Guid id, bool includeDetails = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        var listing = await GetListingInternalAsync(connection, "l.Id = @Id", new { Id = id });
        if (listing is null || !includeDetails) return listing;

        await LoadDetailsAsync(connection, listing);
        return listing;
    }

    public async Task<Listing?> GetBySlugAsync(string slug, bool includeDetails = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        var listing = await GetListingInternalAsync(connection, "l.Slug = @Slug", new { Slug = slug.ToLowerInvariant() });
        if (listing is null || !includeDetails) return listing;

        await LoadDetailsAsync(connection, listing);
        return listing;
    }

    public async Task<(IReadOnlyList<Listing> Items, int TotalCount)> SearchAsync(ListingSearchRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        var where = new StringBuilder("WHERE l.Status <> @Deleted");
        var parameters = new DynamicParameters();
        parameters.Add("Deleted", (int)ListingStatus.Deleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            where.Append(" AND (l.Title LIKE @SearchTerm OR l.Description LIKE @SearchTerm)");
            parameters.Add("SearchTerm", $"%{request.SearchTerm.Trim()}%");
        }

        if (request.CategoryId.HasValue)
        {
            where.Append(" AND l.CategoryId = @CategoryId");
            parameters.Add("CategoryId", request.CategoryId.Value);
        }

        if (request.LocationId.HasValue)
        {
            where.Append(" AND l.LocationId = @LocationId");
            parameters.Add("LocationId", request.LocationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            where.Append(" AND l.City = @City");
            parameters.Add("City", request.City.Trim());
        }

        if (request.MinPrice.HasValue)
        {
            where.Append(" AND l.Price >= @MinPrice");
            parameters.Add("MinPrice", request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            where.Append(" AND l.Price <= @MaxPrice");
            parameters.Add("MaxPrice", request.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<ListingStatus>(request.Status, true, out var status))
        {
            where.Append(" AND l.Status = @Status");
            parameters.Add("Status", (int)status);
        }
        else
        {
            where.Append(" AND l.Status = @ActiveStatus");
            parameters.Add("ActiveStatus", (int)ListingStatus.Active);
        }

        if (request.IsFeatured == true)
        {
            where.Append(" AND l.IsFeatured = 1 AND (l.FeaturedUntil IS NULL OR l.FeaturedUntil > @Now)");
            parameters.Add("Now", DateTime.UtcNow);
        }

        if (request.SellerId.HasValue)
        {
            where.Append(" AND l.SellerId = @SellerId");
            parameters.Add("SellerId", request.SellerId.Value);
        }

        var orderBy = request.SortBy?.ToLowerInvariant() switch
        {
            "price_asc" => "l.Price ASC",
            "price_desc" => "l.Price DESC",
            "oldest" => "l.CreatedAt ASC",
            "views" => "l.ViewCount DESC",
            _ => "l.CreatedAt DESC"
        };

        var offset = (request.Page - 1) * request.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        var countSql = $"SELECT COUNT(1) FROM Listings l {where}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var sql = $@"
            SELECT l.*, c.Id AS SplitCategoryId, c.Name, c.Slug, c.Description, c.IconUrl, c.ParentCategoryId, c.SortOrder, c.IsActive, c.PerDayPriceSale, c.PerDayPriceBuy, c.PerDayPriceRent, c.CreatedAt, c.UpdatedAt
            FROM Listings l
            INNER JOIN Categories c ON c.Id = l.CategoryId
            {where}
            ORDER BY {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var listings = (await QueryListingsWithCategoryAsync(connection, sql, parameters)).ToList();

        foreach (var listing in listings)
        {
            var images = await GetImagesInternalAsync(connection, listing.Id);
            listing.LoadImages(images);
        }

        return (listings, totalCount);
    }

    public async Task<IReadOnlyList<Listing>> GetFeaturedAsync(int limit)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = $@"
            SELECT TOP (@Limit) l.*, c.Id AS SplitCategoryId, c.Name, c.Slug, c.Description, c.IconUrl, c.ParentCategoryId, c.SortOrder, c.IsActive, c.PerDayPriceSale, c.PerDayPriceBuy, c.PerDayPriceRent, c.CreatedAt, c.UpdatedAt
            FROM Listings l
            INNER JOIN Categories c ON c.Id = l.CategoryId
            WHERE l.IsFeatured = 1 AND l.Status = @ActiveStatus
              AND (l.FeaturedUntil IS NULL OR l.FeaturedUntil > @Now)
            ORDER BY l.CreatedAt DESC";

        var listings = (await QueryListingsWithCategoryAsync(connection, sql,
            new { Limit = limit, ActiveStatus = (int)ListingStatus.Active, Now = DateTime.UtcNow })).ToList();

        foreach (var listing in listings)
        {
            var images = await GetImagesInternalAsync(connection, listing.Id);
            listing.LoadImages(images);
        }

        return listings;
    }

    public async Task<IReadOnlyList<Listing>> GetBySellerIdAsync(Guid sellerId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT l.*, c.Id AS SplitCategoryId, c.Name, c.Slug, c.Description, c.IconUrl, c.ParentCategoryId, c.SortOrder, c.IsActive, c.PerDayPriceSale, c.PerDayPriceBuy, c.PerDayPriceRent, c.CreatedAt, c.UpdatedAt
            FROM Listings l
            INNER JOIN Categories c ON c.Id = l.CategoryId
            WHERE l.SellerId = @SellerId AND l.Status <> @Deleted
            ORDER BY l.CreatedAt DESC";

        var listings = (await QueryListingsWithCategoryAsync(connection, sql,
            new { SellerId = sellerId, Deleted = (int)ListingStatus.Deleted })).ToList();

        foreach (var listing in listings)
        {
            var images = await GetImagesInternalAsync(connection, listing.Id);
            listing.LoadImages(images);
        }

        return listings;
    }

    public async Task AddAsync(Listing listing)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await InsertListingAsync(connection, listing, transaction);

            foreach (var image in listing.Images)
            {
                await InsertImageAsync(connection, image, transaction);
            }

            foreach (var attr in listing.AttributeValues)
            {
                await InsertAttributeValueAsync(connection, attr, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateAsync(Listing listing)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            listing.MarkAsUpdated();

            await connection.ExecuteAsync(@"
                UPDATE Listings SET
                    CategoryId = @CategoryId, Title = @Title, Slug = @Slug, Description = @Description,
                    Price = @Price, Currency = @Currency, PriceType = @PriceType, Condition = @Condition, Status = @Status,
                    LocationId = @LocationId, City = @City, District = @District, Province = @Province, Country = @Country,
                    Latitude = @Latitude, Longitude = @Longitude,
                    ContactPhone = @ContactPhone, ContactEmail = @ContactEmail, ShowPhone = @ShowPhone, ShowEmail = @ShowEmail,
                    ViewCount = @ViewCount, IsFeatured = @IsFeatured, FeaturedUntil = @FeaturedUntil,
                    PublishedAt = @PublishedAt, ExpiresAt = @ExpiresAt,
                    ListingPurpose = @ListingPurpose, MobilePhone = @MobilePhone, WhatsAppPhone = @WhatsAppPhone,
                    Address = @Address, AdDurationDays = @AdDurationDays, PaymentAmount = @PaymentAmount,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id",
                MapListingParams(listing), transaction);

            await connection.ExecuteAsync(
                "DELETE FROM ListingImages WHERE ListingId = @ListingId",
                new { ListingId = listing.Id }, transaction);

            foreach (var image in listing.Images)
            {
                await InsertImageAsync(connection, image, transaction);
            }

            await connection.ExecuteAsync(
                "DELETE FROM ListingAttributeValues WHERE ListingId = @ListingId",
                new { ListingId = listing.Id }, transaction);

            foreach (var attr in listing.AttributeValues)
            {
                await InsertAttributeValueAsync(connection, attr, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = excludeId.HasValue
            ? "SELECT COUNT(1) FROM Listings WHERE Slug = @Slug AND Id <> @ExcludeId"
            : "SELECT COUNT(1) FROM Listings WHERE Slug = @Slug";

        return await connection.ExecuteScalarAsync<int>(sql,
            new { Slug = slug.ToLowerInvariant(), ExcludeId = excludeId }) > 0;
    }

    private async Task<Listing?> GetListingInternalAsync(IDbConnection connection, string whereClause, object parameters)
    {
        var sql = $@"
            SELECT l.*, c.Id AS SplitCategoryId, c.Name, c.Slug, c.Description, c.IconUrl, c.ParentCategoryId, c.SortOrder, c.IsActive, c.PerDayPriceSale, c.PerDayPriceBuy, c.PerDayPriceRent, c.CreatedAt, c.UpdatedAt
            FROM Listings l
            INNER JOIN Categories c ON c.Id = l.CategoryId
            WHERE {whereClause}";

        var results = await QueryListingsWithCategoryAsync(connection, sql, parameters);
        return results.FirstOrDefault();
    }

    private static async Task LoadDetailsAsync(IDbConnection connection, Listing listing)
    {
        var images = await GetImagesInternalAsync(connection, listing.Id);
        listing.LoadImages(images);

        var attributeValues = await GetAttributeValuesInternalAsync(connection, listing.Id);
        listing.LoadAttributeValues(attributeValues);

        if (listing.LocationId.HasValue)
        {
            var locationRow = await connection.QuerySingleOrDefaultAsync<LocationFullRow>(
                "SELECT * FROM Locations WHERE Id = @Id", new { Id = listing.LocationId.Value });

            if (locationRow is not null)
            {
                listing.SetLocation(Location.Restore(
                    locationRow.Id, locationRow.Name, locationRow.Slug, (LocationType)locationRow.Type,
                    locationRow.ParentLocationId, locationRow.SortOrder, locationRow.IsActive,
                    locationRow.CreatedAt, locationRow.UpdatedAt));
            }
        }
    }

    private static async Task<IEnumerable<Listing>> QueryListingsWithCategoryAsync(
        IDbConnection connection, string sql, object? parameters)
    {
        var listingMap = new Dictionary<Guid, Listing>();

        await connection.QueryAsync<ListingRow, CategoryRow, Listing>(
            sql,
            (listingRow, categoryRow) =>
            {
                if (!listingMap.TryGetValue(listingRow.Id, out var listing))
                {
                    listing = MapListing(listingRow);
                    listing.SetCategory(Category.Restore(
                        categoryRow.SplitCategoryId, categoryRow.Name, categoryRow.Slug, categoryRow.Description,
                        categoryRow.IconUrl, categoryRow.ParentCategoryId, categoryRow.SortOrder,
                        categoryRow.IsActive,
                        categoryRow.PerDayPriceSale, categoryRow.PerDayPriceBuy, categoryRow.PerDayPriceRent,
                        categoryRow.CreatedAt, categoryRow.UpdatedAt));
                    listingMap.Add(listing.Id, listing);
                }

                return listing;
            },
            parameters,
            splitOn: "SplitCategoryId");

        return listingMap.Values;
    }

    private static async Task<IReadOnlyList<ListingImage>> GetImagesInternalAsync(IDbConnection connection, Guid listingId)
    {
        var rows = await connection.QueryAsync<ListingImageRow>(
            "SELECT * FROM ListingImages WHERE ListingId = @ListingId ORDER BY SortOrder",
            new { ListingId = listingId });

        return rows.Select(r => ListingImage.Restore(
            r.Id, r.ListingId, r.Url, r.ThumbnailUrl, r.AltText, r.SortOrder, r.IsPrimary, r.CreatedAt, r.UpdatedAt)).ToList();
    }

    private static async Task<IReadOnlyList<ListingAttributeValue>> GetAttributeValuesInternalAsync(IDbConnection connection, Guid listingId)
    {
        var sql = @"
            SELECT v.*, a.Id AS SplitAttributeId, a.CategoryId, a.Name, a.DisplayName, a.FieldType, a.Options, a.IsRequired, a.IsFilterable, a.SortOrder, a.CreatedAt, a.UpdatedAt
            FROM ListingAttributeValues v
            INNER JOIN CategoryAttributes a ON a.Id = v.CategoryAttributeId
            WHERE v.ListingId = @ListingId";

        var results = new List<ListingAttributeValue>();

        await connection.QueryAsync<ListingAttributeValueRow, CategoryAttributeRow, ListingAttributeValue>(
            sql,
            (valueRow, attrRow) =>
            {
                var value = ListingAttributeValue.Restore(
                    valueRow.Id, valueRow.ListingId, valueRow.CategoryAttributeId, valueRow.Value,
                    valueRow.CreatedAt, valueRow.UpdatedAt);
                value.SetCategoryAttribute(CategoryAttribute.Restore(
                    attrRow.SplitAttributeId, attrRow.CategoryId, attrRow.Name, attrRow.DisplayName,
                    (AttributeFieldType)attrRow.FieldType, attrRow.Options, attrRow.IsRequired,
                    attrRow.IsFilterable, attrRow.SortOrder, attrRow.CreatedAt, attrRow.UpdatedAt));
                results.Add(value);
                return value;
            },
            new { ListingId = listingId },
            splitOn: "SplitAttributeId");

        return results;
    }

    private static async Task InsertListingAsync(IDbConnection connection, Listing listing, IDbTransaction transaction)
    {
        await connection.ExecuteAsync(@"
            INSERT INTO Listings (
                Id, SellerId, CategoryId, Title, Slug, Description, Price, Currency, PriceType, Condition, Status,
                LocationId, City, District, Province, Country, Latitude, Longitude,
                ContactPhone, ContactEmail, ShowPhone, ShowEmail, ViewCount, IsFeatured, FeaturedUntil,
                PublishedAt, ExpiresAt, ListingPurpose, MobilePhone, WhatsAppPhone, Address, AdDurationDays, PaymentAmount,
                CreatedAt, UpdatedAt)
            VALUES (
                @Id, @SellerId, @CategoryId, @Title, @Slug, @Description, @Price, @Currency, @PriceType, @Condition, @Status,
                @LocationId, @City, @District, @Province, @Country, @Latitude, @Longitude,
                @ContactPhone, @ContactEmail, @ShowPhone, @ShowEmail, @ViewCount, @IsFeatured, @FeaturedUntil,
                @PublishedAt, @ExpiresAt, @ListingPurpose, @MobilePhone, @WhatsAppPhone, @Address, @AdDurationDays, @PaymentAmount,
                @CreatedAt, @UpdatedAt)",
            MapListingParams(listing), transaction);
    }

    private static async Task InsertImageAsync(IDbConnection connection, ListingImage image, IDbTransaction transaction)
    {
        await connection.ExecuteAsync(@"
            INSERT INTO ListingImages (Id, ListingId, Url, ThumbnailUrl, AltText, SortOrder, IsPrimary, CreatedAt, UpdatedAt)
            VALUES (@Id, @ListingId, @Url, @ThumbnailUrl, @AltText, @SortOrder, @IsPrimary, @CreatedAt, @UpdatedAt)",
            new
            {
                image.Id,
                image.ListingId,
                image.Url,
                image.ThumbnailUrl,
                image.AltText,
                image.SortOrder,
                image.IsPrimary,
                image.CreatedAt,
                image.UpdatedAt
            }, transaction);
    }

    private static async Task InsertAttributeValueAsync(IDbConnection connection, ListingAttributeValue attr, IDbTransaction transaction)
    {
        await connection.ExecuteAsync(@"
            INSERT INTO ListingAttributeValues (Id, ListingId, CategoryAttributeId, Value, CreatedAt, UpdatedAt)
            VALUES (@Id, @ListingId, @CategoryAttributeId, @Value, @CreatedAt, @UpdatedAt)",
            new
            {
                attr.Id,
                attr.ListingId,
                attr.CategoryAttributeId,
                attr.Value,
                attr.CreatedAt,
                attr.UpdatedAt
            }, transaction);
    }

    private static Listing MapListing(ListingRow row) =>
        Listing.Restore(
            row.Id, row.SellerId, row.CategoryId, row.Title, row.Slug, row.Description,
            row.Price, row.Currency, (PriceType)row.PriceType, (ListingCondition)row.Condition, (ListingStatus)row.Status,
            row.LocationId, row.City, row.District, row.Province, row.Country,
            row.Latitude, row.Longitude, row.ContactPhone, row.ContactEmail, row.ShowPhone, row.ShowEmail,
            row.ViewCount, row.IsFeatured, row.FeaturedUntil, row.PublishedAt, row.ExpiresAt,
            row.ListingPurpose, row.MobilePhone, row.WhatsAppPhone, row.Address, row.AdDurationDays, row.PaymentAmount,
            row.CreatedAt, row.UpdatedAt);

    private static object MapListingParams(Listing listing) => new
    {
        listing.Id,
        listing.SellerId,
        listing.CategoryId,
        listing.Title,
        listing.Slug,
        listing.Description,
        listing.Price,
        listing.Currency,
        PriceType = (int)listing.PriceType,
        Condition = (int)listing.Condition,
        Status = (int)listing.Status,
        listing.LocationId,
        listing.City,
        listing.District,
        listing.Province,
        listing.Country,
        listing.Latitude,
        listing.Longitude,
        listing.ContactPhone,
        listing.ContactEmail,
        listing.ShowPhone,
        listing.ShowEmail,
        listing.ViewCount,
        listing.IsFeatured,
        listing.FeaturedUntil,
        listing.PublishedAt,
        listing.ExpiresAt,
        listing.ListingPurpose,
        listing.MobilePhone,
        listing.WhatsAppPhone,
        listing.Address,
        listing.AdDurationDays,
        listing.PaymentAmount,
        listing.CreatedAt,
        listing.UpdatedAt
    };

    private class ListingRow
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Currency { get; set; } = "LKR";
        public int PriceType { get; set; }
        public int Condition { get; set; }
        public int Status { get; set; }
        public Guid? LocationId { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Province { get; set; }
        public string Country { get; set; } = "Sri Lanka";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowEmail { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime? FeaturedUntil { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? ListingPurpose { get; set; }
        public string? MobilePhone { get; set; }
        public string? WhatsAppPhone { get; set; }
        public string? Address { get; set; }
        public int AdDurationDays { get; set; } = 30;
        public decimal? PaymentAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class CategoryRow
    {
        public Guid SplitCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public decimal PerDayPriceSale { get; set; }
        public decimal PerDayPriceBuy { get; set; }
        public decimal PerDayPriceRent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class ListingImageRow
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class ListingAttributeValueRow
    {
        public Guid Id { get; set; }
        public Guid ListingId { get; set; }
        public Guid CategoryAttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class CategoryAttributeRow
    {
        public Guid SplitAttributeId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int FieldType { get; set; }
        public string? Options { get; set; }
        public bool IsRequired { get; set; }
        public bool IsFilterable { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class LocationFullRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int Type { get; set; }
        public Guid? ParentLocationId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
