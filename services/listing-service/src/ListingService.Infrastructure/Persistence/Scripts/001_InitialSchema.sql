IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SchemaVersions')
BEGIN
    CREATE TABLE SchemaVersions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ScriptName NVARCHAR(200) NOT NULL UNIQUE,
        AppliedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Slug NVARCHAR(150) NOT NULL,
        Description NVARCHAR(500) NULL,
        IconUrl NVARCHAR(512) NULL,
        ParentCategoryId UNIQUEIDENTIFIER NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES Categories(Id)
    );
    CREATE UNIQUE INDEX IX_Categories_Slug ON Categories(Slug);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CategoryAttributes')
BEGIN
    CREATE TABLE CategoryAttributes (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        CategoryId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        DisplayName NVARCHAR(150) NOT NULL,
        FieldType INT NOT NULL,
        Options NVARCHAR(2000) NULL,
        IsRequired BIT NOT NULL DEFAULT 0,
        IsFilterable BIT NOT NULL DEFAULT 0,
        SortOrder INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_CategoryAttributes_Category FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
    );
    CREATE UNIQUE INDEX IX_CategoryAttributes_CategoryId_Name ON CategoryAttributes(CategoryId, Name);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Locations')
BEGIN
    CREATE TABLE Locations (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Slug NVARCHAR(150) NOT NULL,
        Type INT NOT NULL,
        ParentLocationId UNIQUEIDENTIFIER NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Locations_Parent FOREIGN KEY (ParentLocationId) REFERENCES Locations(Id)
    );
    CREATE UNIQUE INDEX IX_Locations_Slug_Parent ON Locations(Slug, ParentLocationId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Listings')
BEGIN
    CREATE TABLE Listings (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        SellerId UNIQUEIDENTIFIER NOT NULL,
        CategoryId UNIQUEIDENTIFIER NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Slug NVARCHAR(250) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Price DECIMAL(18,2) NULL,
        Currency NVARCHAR(10) NOT NULL DEFAULT 'LKR',
        PriceType INT NOT NULL DEFAULT 0,
        Condition INT NOT NULL DEFAULT 0,
        Status INT NOT NULL DEFAULT 0,
        LocationId UNIQUEIDENTIFIER NULL,
        City NVARCHAR(100) NULL,
        District NVARCHAR(100) NULL,
        Province NVARCHAR(100) NULL,
        Country NVARCHAR(100) NOT NULL DEFAULT 'Sri Lanka',
        Latitude FLOAT NULL,
        Longitude FLOAT NULL,
        ContactPhone NVARCHAR(20) NULL,
        ContactEmail NVARCHAR(256) NULL,
        ShowPhone BIT NOT NULL DEFAULT 1,
        ShowEmail BIT NOT NULL DEFAULT 0,
        ViewCount INT NOT NULL DEFAULT 0,
        IsFeatured BIT NOT NULL DEFAULT 0,
        FeaturedUntil DATETIME2 NULL,
        PublishedAt DATETIME2 NULL,
        ExpiresAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Listings_Category FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
        CONSTRAINT FK_Listings_Location FOREIGN KEY (LocationId) REFERENCES Locations(Id)
    );
    CREATE UNIQUE INDEX IX_Listings_Slug ON Listings(Slug);
    CREATE INDEX IX_Listings_SellerId ON Listings(SellerId);
    CREATE INDEX IX_Listings_CategoryId ON Listings(CategoryId);
    CREATE INDEX IX_Listings_Status ON Listings(Status);
    CREATE INDEX IX_Listings_IsFeatured ON Listings(IsFeatured);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ListingImages')
BEGIN
    CREATE TABLE ListingImages (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        ListingId UNIQUEIDENTIFIER NOT NULL,
        Url NVARCHAR(512) NOT NULL,
        ThumbnailUrl NVARCHAR(512) NULL,
        AltText NVARCHAR(200) NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        IsPrimary BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_ListingImages_Listing FOREIGN KEY (ListingId) REFERENCES Listings(Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ListingAttributeValues')
BEGIN
    CREATE TABLE ListingAttributeValues (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        ListingId UNIQUEIDENTIFIER NOT NULL,
        CategoryAttributeId UNIQUEIDENTIFIER NOT NULL,
        Value NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_ListingAttributeValues_Listing FOREIGN KEY (ListingId) REFERENCES Listings(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ListingAttributeValues_Attribute FOREIGN KEY (CategoryAttributeId) REFERENCES CategoryAttributes(Id)
    );
    CREATE UNIQUE INDEX IX_ListingAttributeValues_Listing_Attribute ON ListingAttributeValues(ListingId, CategoryAttributeId);
END
GO
