using Dapper;
using ListingService.Application.Interfaces;
using ListingService.Application.DTOs.Payments;
using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CategoryRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Category?> GetByIdAsync(Guid id, bool includeAttributes = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        var row = await connection.QuerySingleOrDefaultAsync<CategoryRow>(
            "SELECT * FROM Categories WHERE Id = @Id", new { Id = id });

        if (row is null) return null;

        var category = MapCategory(row);

        if (includeAttributes)
        {
            var attributes = await GetAttributesInternalAsync(connection, id);
            category.LoadAttributes(attributes);
        }

        return category;
    }

    public async Task<Category?> GetBySlugAsync(string slug, bool includeAttributes = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        var row = await connection.QuerySingleOrDefaultAsync<CategoryRow>(
            "SELECT * FROM Categories WHERE Slug = @Slug", new { Slug = slug.ToLowerInvariant() });

        if (row is null) return null;

        var category = MapCategory(row);

        if (includeAttributes)
        {
            var attributes = await GetAttributesInternalAsync(connection, category.Id);
            category.LoadAttributes(attributes);
        }

        return category;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(bool activeOnly = true)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = activeOnly
            ? "SELECT * FROM Categories WHERE IsActive = 1 ORDER BY SortOrder"
            : "SELECT * FROM Categories ORDER BY SortOrder";

        var rows = await connection.QueryAsync<CategoryRow>(sql);
        return rows.Select(MapCategory).ToList();
    }

    public async Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentCategoryId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var rows = await connection.QueryAsync<CategoryRow>(
            "SELECT * FROM Categories WHERE ParentCategoryId = @ParentCategoryId AND IsActive = 1 ORDER BY SortOrder",
            new { ParentCategoryId = parentCategoryId });

        return rows.Select(MapCategory).ToList();
    }

    public async Task<IReadOnlyList<CategoryAttribute>> GetAttributesByCategoryIdAsync(Guid categoryId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await GetAttributesInternalAsync(connection, categoryId);
    }

    public async Task AddAsync(Category category)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(@"
            INSERT INTO Categories (Id, Name, Slug, Description, IconUrl, ParentCategoryId, SortOrder, IsActive, CreatedAt, UpdatedAt)
            VALUES (@Id, @Name, @Slug, @Description, @IconUrl, @ParentCategoryId, @SortOrder, @IsActive, @CreatedAt, @UpdatedAt)",
            MapCategoryParams(category));

        foreach (var attribute in category.Attributes)
        {
            await InsertAttributeAsync(connection, attribute);
        }
    }

    public async Task UpdateAsync(Category category)
    {
        using var connection = _connectionFactory.CreateConnection();
        category.MarkAsUpdated();

        await connection.ExecuteAsync(@"
            UPDATE Categories SET
                Name = @Name, Slug = @Slug, Description = @Description, IconUrl = @IconUrl,
                SortOrder = @SortOrder, IsActive = @IsActive, UpdatedAt = @UpdatedAt
            WHERE Id = @Id",
            MapCategoryParams(category));

        foreach (var attribute in category.Attributes)
        {
            var exists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM CategoryAttributes WHERE Id = @Id", new { attribute.Id });

            if (exists == 0)
            {
                await InsertAttributeAsync(connection, attribute);
            }
            else
            {
                await connection.ExecuteAsync(@"
                    UPDATE CategoryAttributes SET
                        DisplayName = @DisplayName, FieldType = @FieldType, Options = @Options,
                        IsRequired = @IsRequired, IsFilterable = @IsFilterable, SortOrder = @SortOrder, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id",
                    new
                    {
                        attribute.Id,
                        attribute.DisplayName,
                        FieldType = (int)attribute.FieldType,
                        attribute.Options,
                        attribute.IsRequired,
                        attribute.IsFilterable,
                        attribute.SortOrder,
                        UpdatedAt = DateTime.UtcNow
                    });
            }
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = excludeId.HasValue
            ? "SELECT COUNT(1) FROM Categories WHERE Slug = @Slug AND Id <> @ExcludeId"
            : "SELECT COUNT(1) FROM Categories WHERE Slug = @Slug";

        return await connection.ExecuteScalarAsync<int>(sql, new { Slug = slug.ToLowerInvariant(), ExcludeId = excludeId }) > 0;
    }

    public async Task<int> GetListingCountAsync(Guid categoryId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(@"
            SELECT COUNT(1) FROM Listings
            WHERE CategoryId = @CategoryId AND Status NOT IN (@Deleted, @Sold)",
            new { CategoryId = categoryId, Deleted = (int)ListingStatus.Deleted, Sold = (int)ListingStatus.Sold });
    }

    public async Task<IReadOnlyList<CategoryPricingResponse>> GetRootCategoryPricingAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<CategoryPricingRow>(@"
            SELECT Id, Name, Slug, PerDayPriceSale, PerDayPriceBuy, PerDayPriceRent
            FROM Categories
            WHERE ParentCategoryId IS NULL AND IsActive = 1
            ORDER BY SortOrder");

        return rows.Select(r => new CategoryPricingResponse(
            r.Id, r.Name, r.Slug, r.PerDayPriceSale, r.PerDayPriceBuy, r.PerDayPriceRent)).ToList();
    }

    public async Task<CategoryPricingResponse?> UpdatePricingAsync(
        Guid categoryId, decimal sale, decimal buy, decimal rent)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(@"
            UPDATE Categories SET
                PerDayPriceSale = @Sale, PerDayPriceBuy = @Buy, PerDayPriceRent = @Rent, UpdatedAt = @Now
            WHERE Id = @Id AND ParentCategoryId IS NULL",
            new { Id = categoryId, Sale = sale, Buy = buy, Rent = rent, Now = DateTime.UtcNow });

        if (rows == 0)
        {
            return null;
        }

        return await connection.QuerySingleOrDefaultAsync<CategoryPricingResponse>(@"
            SELECT Id, Name, Slug, PerDayPriceSale, PerDayPriceBuy, PerDayPriceRent
            FROM Categories WHERE Id = @Id",
            new { Id = categoryId });
    }

    private class CategoryPricingRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal PerDayPriceSale { get; set; }
        public decimal PerDayPriceBuy { get; set; }
        public decimal PerDayPriceRent { get; set; }
    }

    private static async Task<IReadOnlyList<CategoryAttribute>> GetAttributesInternalAsync(System.Data.IDbConnection connection, Guid categoryId)
    {
        var rows = await connection.QueryAsync<CategoryAttributeRow>(
            "SELECT * FROM CategoryAttributes WHERE CategoryId = @CategoryId ORDER BY SortOrder",
            new { CategoryId = categoryId });

        return rows.Select(MapAttribute).ToList();
    }

    private static async Task InsertAttributeAsync(System.Data.IDbConnection connection, CategoryAttribute attribute)
    {
        await connection.ExecuteAsync(@"
            INSERT INTO CategoryAttributes (Id, CategoryId, Name, DisplayName, FieldType, Options, IsRequired, IsFilterable, SortOrder, CreatedAt, UpdatedAt)
            VALUES (@Id, @CategoryId, @Name, @DisplayName, @FieldType, @Options, @IsRequired, @IsFilterable, @SortOrder, @CreatedAt, @UpdatedAt)",
            new
            {
                attribute.Id,
                attribute.CategoryId,
                attribute.Name,
                attribute.DisplayName,
                FieldType = (int)attribute.FieldType,
                attribute.Options,
                attribute.IsRequired,
                attribute.IsFilterable,
                attribute.SortOrder,
                attribute.CreatedAt,
                attribute.UpdatedAt
            });
    }

    private static Category MapCategory(CategoryRow row) =>
        Category.Restore(row.Id, row.Name, row.Slug, row.Description, row.IconUrl,
            row.ParentCategoryId, row.SortOrder, row.IsActive,
            row.PerDayPriceSale, row.PerDayPriceBuy, row.PerDayPriceRent,
            row.CreatedAt, row.UpdatedAt);

    private static CategoryAttribute MapAttribute(CategoryAttributeRow row) =>
        CategoryAttribute.Restore(row.Id, row.CategoryId, row.Name, row.DisplayName,
            (AttributeFieldType)row.FieldType, row.Options, row.IsRequired, row.IsFilterable,
            row.SortOrder, row.CreatedAt, row.UpdatedAt);

    private static object MapCategoryParams(Category category) => new
    {
        category.Id,
        category.Name,
        category.Slug,
        category.Description,
        category.IconUrl,
        category.ParentCategoryId,
        category.SortOrder,
        category.IsActive,
        category.CreatedAt,
        category.UpdatedAt
    };

    private class CategoryRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public decimal PerDayPriceSale { get; set; } = 50;
        public decimal PerDayPriceBuy { get; set; } = 30;
        public decimal PerDayPriceRent { get; set; } = 40;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class CategoryAttributeRow
    {
        public Guid Id { get; set; }
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
}
