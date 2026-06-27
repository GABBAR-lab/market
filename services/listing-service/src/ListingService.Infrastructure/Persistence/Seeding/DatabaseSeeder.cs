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
        var categoryIds = new Dictionary<string, Guid>();
        var now = DateTime.UtcNow;

        if (categoryCount == 0)
        {
            await SeedCategoriesAndLocationsAsync(connection, categoryIds, now);
        }
        else
        {
            var rows = await connection.QueryAsync<(string Slug, Guid Id)>("SELECT Slug, Id FROM Categories");
            foreach (var (slug, id) in rows)
            {
                categoryIds[slug] = id;
            }

            await EnsureCategoryIconsAsync(connection, categoryIds);
        }

        await EnsureSubCategoriesAsync(connection, categoryIds, now);
        await SeedSampleListingsAsync(connection, categoryIds, now);
    }

    private static async Task SeedCategoriesAndLocationsAsync(
        IDbConnection connection,
        Dictionary<string, Guid> categoryIds,
        DateTime now)
    {
        var categories = new (string Name, string Slug, string IconKey, int Sort)[]
        {
            ("Vehicles", "vehicles", "vehicles", 1),
            ("Property", "property", "property", 2),
            ("Mobiles", "mobiles", "mobiles", 3),
            ("Electronics", "electronics", "electronics", 4),
            ("Services", "services", "services", 5),
            ("Home & Garden", "home-garden", "home-garden", 6),
            ("Business & Industry", "business-industry", "business-industry", 7),
            ("Jobs", "jobs", "jobs", 8),
            ("Animals", "animals", "animals", 9),
            ("Hobby, Sport & Kids", "hobby-sport-kids", "hobby-sport-kids", 10),
            ("Fashion & Beauty", "fashion-beauty", "fashion-beauty", 11),
            ("Essentials", "essentials", "essentials", 12),
            ("Education", "education", "education", 13),
            ("Agriculture", "agriculture", "agriculture", 14),
            ("Work Overseas", "work-overseas", "work-overseas", 15),
            ("Other", "other", "other", 16)
        };

        foreach (var (name, slug, iconKey, sort) in categories)
        {
            var id = Guid.NewGuid();
            categoryIds[slug] = id;

            await connection.ExecuteAsync(@"
                INSERT INTO Categories (Id, Name, Slug, IconUrl, SortOrder, IsActive, CreatedAt)
                VALUES (@Id, @Name, @Slug, @IconUrl, @SortOrder, 1, @CreatedAt)",
                new { Id = id, Name = name, Slug = slug, IconUrl = iconKey, SortOrder = sort, CreatedAt = now });
        }

        await EnsureSubCategoriesAsync(connection, categoryIds, now);

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

    private static async Task SeedSampleListingsAsync(
        IDbConnection connection,
        Dictionary<string, Guid> categoryIds,
        DateTime now)
    {
        var listingCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Listings");
        if (listingCount > 0)
        {
            return;
        }

        var sellerId = SeedUserIds.Seller;
        var featuredUntil = now.AddMonths(3);

        var samples = new (string Title, string Slug, string Category, decimal Price, string City, string District, string Desc, string Image, bool Featured)[]
        {
            (
                "3 Bedroom House For Sale in Colombo 05",
                "3-bedroom-house-colombo-05",
                "property",
                4500000m,
                "Colombo 05",
                "Colombo",
                "Beautiful 3 bedroom house with garden, parking, and modern kitchen. Close to schools and shopping.",
                "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800&q=80",
                true
            ),
            (
                "Luxury Villa With Pool in Galle",
                "luxury-villa-pool-galle",
                "property",
                12500000m,
                "Galle",
                "Galle",
                "Stunning beach-side villa with private pool, 4 bedrooms, and ocean views.",
                "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800&q=80",
                true
            ),
            (
                "Modern Apartment For Rent in Kandy",
                "modern-apartment-rent-kandy",
                "property",
                85000m,
                "Kandy",
                "Kandy",
                "Fully furnished 2 bedroom apartment in central Kandy. Available immediately.",
                "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800&q=80",
                true
            ),
            (
                "Toyota Prius 2018 — Low Mileage",
                "toyota-prius-2018",
                "vehicles",
                7200000m,
                "Colombo 03",
                "Colombo",
                "Well maintained Toyota Prius 2018, single owner, full service history.",
                "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800&q=80",
                true
            ),
            (
                "iPhone 15 Pro Max 256GB",
                "iphone-15-pro-max-256gb",
                "mobiles",
                385000m,
                "Colombo 07",
                "Colombo",
                "Brand new sealed iPhone 15 Pro Max, 256GB, with official warranty.",
                "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=800&q=80",
                false
            ),
            (
                "Samsung 55\" Smart TV",
                "samsung-55-smart-tv",
                "electronics",
                175000m,
                "Negombo",
                "Gampaha",
                "Samsung 55 inch 4K Smart TV, barely used, with remote and wall mount.",
                "https://images.unsplash.com/photo-1593359677873-a4bb92f829e1?w=800&q=80",
                false
            ),
            (
                "Office Space For Rent — Colombo 01",
                "office-space-rent-colombo-01",
                "property",
                120000m,
                "Colombo 01",
                "Colombo",
                "Prime commercial office space, 1200 sq.ft, fully air-conditioned.",
                "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&q=80",
                false
            ),
            (
                "Honda CB250F Motorcycle",
                "honda-cb250f-motorcycle",
                "vehicles",
                890000m,
                "Kalutara",
                "Kalutara",
                "Honda CB250F in excellent condition, recently serviced.",
                "https://images.unsplash.com/photo-1558981403-c5f9899a28bc?w=800&q=80",
                false
            ),
        };

        foreach (var (title, slug, category, price, city, district, desc, image, featured) in samples)
        {
            var listingId = Guid.NewGuid();
            var imageId = Guid.NewGuid();
            var publishedAt = now.AddDays(-Random.Shared.Next(1, 14));

            await connection.ExecuteAsync(@"
                INSERT INTO Listings (
                    Id, SellerId, CategoryId, Title, Slug, Description, Price, Currency,
                    PriceType, Condition, Status, City, District, Province, Country,
                    ContactPhone, ContactEmail, ShowPhone, ShowEmail, ViewCount,
                    IsFeatured, FeaturedUntil, PublishedAt, CreatedAt)
                VALUES (
                    @Id, @SellerId, @CategoryId, @Title, @Slug, @Description, @Price, 'LKR',
                    @PriceType, @Condition, @Status, @City, @District, 'Western Province', 'Sri Lanka',
                    '0771234567', 'seller@marketplace.com', 1, 0, @ViewCount,
                    @IsFeatured, @FeaturedUntil, @PublishedAt, @CreatedAt)",
                new
                {
                    Id = listingId,
                    SellerId = sellerId,
                    CategoryId = categoryIds[category],
                    Title = title,
                    Slug = slug,
                    Description = desc,
                    Price = price,
                    PriceType = (int)PriceType.Fixed,
                    Condition = (int)ListingCondition.Used,
                    Status = (int)ListingStatus.Active,
                    City = city,
                    District = district,
                    ViewCount = Random.Shared.Next(10, 500),
                    IsFeatured = featured,
                    FeaturedUntil = featured ? featuredUntil : (DateTime?)null,
                    PublishedAt = publishedAt,
                    CreatedAt = publishedAt
                });

            await connection.ExecuteAsync(@"
                INSERT INTO ListingImages (Id, ListingId, Url, AltText, SortOrder, IsPrimary, CreatedAt)
                VALUES (@Id, @ListingId, @Url, @AltText, 0, 1, @CreatedAt)",
                new
                {
                    Id = imageId,
                    ListingId = listingId,
                    Url = image,
                    AltText = title,
                    CreatedAt = publishedAt
                });
        }
    }

    private static async Task EnsureCategoryIconsAsync(
        IDbConnection connection,
        Dictionary<string, Guid> categoryIds)
    {
        foreach (var (slug, iconKey) in CategorySubcategorySeedData.ParentIconKeys)
        {
            if (!categoryIds.ContainsKey(slug))
            {
                continue;
            }

            await connection.ExecuteAsync(@"
                UPDATE Categories SET IconUrl = @IconUrl
                WHERE Slug = @Slug AND (IconUrl IS NULL OR IconUrl LIKE '/icons/%')",
                new { Slug = slug, IconUrl = iconKey });
        }
    }

    private static async Task EnsureSubCategoriesAsync(
        IDbConnection connection,
        Dictionary<string, Guid> categoryIds,
        DateTime now)
    {
        var subCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Categories WHERE ParentCategoryId IS NOT NULL");

        if (subCount > 0)
        {
            return;
        }

        foreach (var (parentSlug, subcategories) in CategorySubcategorySeedData.ByParent)
        {
            if (!categoryIds.TryGetValue(parentSlug, out var parentId))
            {
                continue;
            }

            foreach (var sub in subcategories)
            {
                var id = Guid.NewGuid();
                var description = sub.SearchTerm is null ? null : $"search:{sub.SearchTerm}";

                await connection.ExecuteAsync(@"
                    INSERT INTO Categories (Id, Name, Slug, Description, IconUrl, ParentCategoryId, SortOrder, IsActive, CreatedAt)
                    VALUES (@Id, @Name, @Slug, @Description, @IconUrl, @ParentCategoryId, @SortOrder, 1, @CreatedAt)",
                    new
                    {
                        Id = id,
                        sub.Name,
                        sub.Slug,
                        Description = description,
                        IconUrl = sub.IconKey,
                        ParentCategoryId = parentId,
                        SortOrder = sub.Sort,
                        CreatedAt = now
                    });
            }
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
