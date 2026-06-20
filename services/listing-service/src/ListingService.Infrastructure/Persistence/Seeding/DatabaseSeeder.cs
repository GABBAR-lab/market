using System.Data;
using Dapper;
using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IDbConnection connection)
    {
        var categoryCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Categories");
        if (categoryCount > 0)
        {
            return;
        }

        var categories = new (string Name, string Slug, string Icon, int Sort)[]
        {
            ("Vehicles", "vehicles", "/icons/vehicles.svg", 1),
            ("Property", "property", "/icons/property.svg", 2),
            ("Electronics", "electronics", "/icons/electronics.svg", 3),
            ("Mobiles", "mobiles", "/icons/mobiles.svg", 4),
            ("Services", "services", "/icons/services.svg", 5),
            ("Home & Garden", "home-garden", "/icons/home-garden.svg", 6),
            ("Business & Industry", "business-industry", "/icons/business.svg", 7),
            ("Jobs", "jobs", "/icons/jobs.svg", 8),
            ("Animals", "animals", "/icons/animals.svg", 9),
            ("Hobby, Sport & Kids", "hobby-sport-kids", "/icons/hobby.svg", 10),
            ("Fashion & Beauty", "fashion-beauty", "/icons/fashion.svg", 11),
            ("Education", "education", "/icons/education.svg", 12),
            ("Essentials", "essentials", "/icons/essentials.svg", 13),
            ("Agriculture", "agriculture", "/icons/agriculture.svg", 14),
            ("Work Overseas", "work-overseas", "/icons/overseas.svg", 15),
            ("Other", "other", "/icons/other.svg", 16)
        };

        var categoryIds = new Dictionary<string, Guid>();
        var now = DateTime.UtcNow;

        foreach (var (name, slug, icon, sort) in categories)
        {
            var id = Guid.NewGuid();
            categoryIds[slug] = id;

            await connection.ExecuteAsync(@"
                INSERT INTO Categories (Id, Name, Slug, IconUrl, SortOrder, IsActive, CreatedAt)
                VALUES (@Id, @Name, @Slug, @IconUrl, @SortOrder, 1, @CreatedAt)",
                new { Id = id, Name = name, Slug = slug, IconUrl = icon, SortOrder = sort, CreatedAt = now });
        }

        await SeedAttributeAsync(connection, categoryIds["property"], "bedrooms", "Bedrooms", (int)AttributeFieldType.Number, null, false, true, 1, now);
        await SeedAttributeAsync(connection, categoryIds["property"], "bathrooms", "Bathrooms", (int)AttributeFieldType.Number, null, false, true, 2, now);
        await SeedAttributeAsync(connection, categoryIds["property"], "land_size", "Land Size (perches)", (int)AttributeFieldType.Decimal, null, false, true, 3, now);
        await SeedAttributeAsync(connection, categoryIds["property"], "property_type", "Property Type", (int)AttributeFieldType.Select,
            "[\"House\",\"Apartment\",\"Land\",\"Commercial\"]", false, true, 4, now);

        await SeedAttributeAsync(connection, categoryIds["vehicles"], "brand", "Brand", (int)AttributeFieldType.Text, null, false, true, 1, now);
        await SeedAttributeAsync(connection, categoryIds["vehicles"], "model", "Model", (int)AttributeFieldType.Text, null, false, true, 2, now);
        await SeedAttributeAsync(connection, categoryIds["vehicles"], "year", "Year", (int)AttributeFieldType.Number, null, false, true, 3, now);
        await SeedAttributeAsync(connection, categoryIds["vehicles"], "mileage", "Mileage (km)", (int)AttributeFieldType.Number, null, false, true, 4, now);
        await SeedAttributeAsync(connection, categoryIds["vehicles"], "fuel_type", "Fuel Type", (int)AttributeFieldType.Select,
            "[\"Petrol\",\"Diesel\",\"Hybrid\",\"Electric\"]", false, true, 5, now);

        await SeedAttributeAsync(connection, categoryIds["mobiles"], "brand", "Brand", (int)AttributeFieldType.Text, null, false, true, 1, now);
        await SeedAttributeAsync(connection, categoryIds["mobiles"], "model", "Model", (int)AttributeFieldType.Text, null, false, true, 2, now);
        await SeedAttributeAsync(connection, categoryIds["mobiles"], "storage", "Storage", (int)AttributeFieldType.Select,
            "[\"32GB\",\"64GB\",\"128GB\",\"256GB\",\"512GB\"]", false, true, 3, now);

        var countryId = Guid.NewGuid();
        await connection.ExecuteAsync(@"
            INSERT INTO Locations (Id, Name, Slug, Type, SortOrder, IsActive, CreatedAt)
            VALUES (@Id, @Name, @Slug, @Type, @SortOrder, 1, @CreatedAt)",
            new { Id = countryId, Name = "Sri Lanka", Slug = "sri-lanka", Type = (int)LocationType.Country, SortOrder = 1, CreatedAt = now });

        var provinces = new (string Name, string Slug, int Sort)[]
        {
            ("Western Province", "western-province", 1),
            ("Central Province", "central-province", 2),
            ("Southern Province", "southern-province", 3),
            ("Northern Province", "northern-province", 4),
            ("Eastern Province", "eastern-province", 5)
        };

        var provinceIds = new Dictionary<string, Guid>();
        foreach (var (name, slug, sort) in provinces)
        {
            var id = Guid.NewGuid();
            provinceIds[slug] = id;
            await connection.ExecuteAsync(@"
                INSERT INTO Locations (Id, Name, Slug, Type, ParentLocationId, SortOrder, IsActive, CreatedAt)
                VALUES (@Id, @Name, @Slug, @Type, @ParentLocationId, @SortOrder, 1, @CreatedAt)",
                new { Id = id, Name = name, Slug = slug, Type = (int)LocationType.Province, ParentLocationId = countryId, SortOrder = sort, CreatedAt = now });
        }

        var westernId = provinceIds["western-province"];
        var districts = new (string Name, string Slug, int Sort)[]
        {
            ("Colombo", "colombo", 1),
            ("Gampaha", "gampaha", 2),
            ("Kalutara", "kalutara", 3)
        };

        var districtIds = new Dictionary<string, Guid>();
        foreach (var (name, slug, sort) in districts)
        {
            var id = Guid.NewGuid();
            districtIds[slug] = id;
            await connection.ExecuteAsync(@"
                INSERT INTO Locations (Id, Name, Slug, Type, ParentLocationId, SortOrder, IsActive, CreatedAt)
                VALUES (@Id, @Name, @Slug, @Type, @ParentLocationId, @SortOrder, 1, @CreatedAt)",
                new { Id = id, Name = name, Slug = slug, Type = (int)LocationType.District, ParentLocationId = westernId, SortOrder = sort, CreatedAt = now });
        }

        var colomboId = districtIds["colombo"];
        var cities = new (string Name, string Slug, int Sort)[]
        {
            ("Colombo 01", "colombo-01", 1),
            ("Colombo 03", "colombo-03", 2),
            ("Colombo 05", "colombo-05", 3),
            ("Colombo 07", "colombo-07", 4)
        };

        foreach (var (name, slug, sort) in cities)
        {
            await connection.ExecuteAsync(@"
                INSERT INTO Locations (Id, Name, Slug, Type, ParentLocationId, SortOrder, IsActive, CreatedAt)
                VALUES (@Id, @Name, @Slug, @Type, @ParentLocationId, @SortOrder, 1, @CreatedAt)",
                new { Id = Guid.NewGuid(), Name = name, Slug = slug, Type = (int)LocationType.City, ParentLocationId = colomboId, SortOrder = sort, CreatedAt = now });
        }
    }

    private static async Task SeedAttributeAsync(
        IDbConnection connection,
        Guid categoryId,
        string name,
        string displayName,
        int fieldType,
        string? options,
        bool isRequired,
        bool isFilterable,
        int sortOrder,
        DateTime createdAt)
    {
        await connection.ExecuteAsync(@"
            INSERT INTO CategoryAttributes (Id, CategoryId, Name, DisplayName, FieldType, Options, IsRequired, IsFilterable, SortOrder, CreatedAt)
            VALUES (@Id, @CategoryId, @Name, @DisplayName, @FieldType, @Options, @IsRequired, @IsFilterable, @SortOrder, @CreatedAt)",
            new
            {
                Id = Guid.NewGuid(),
                CategoryId = categoryId,
                Name = name,
                DisplayName = displayName,
                FieldType = fieldType,
                Options = options,
                IsRequired = isRequired,
                IsFilterable = isFilterable,
                SortOrder = sortOrder,
                CreatedAt = createdAt
            });
    }
}
