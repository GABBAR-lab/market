IF COL_LENGTH('Categories', 'PerDayPriceSale') IS NULL
    ALTER TABLE Categories ADD PerDayPriceSale DECIMAL(18,2) NOT NULL CONSTRAINT DF_Categories_PerDayPriceSale DEFAULT 50;
GO
IF COL_LENGTH('Categories', 'PerDayPriceBuy') IS NULL
    ALTER TABLE Categories ADD PerDayPriceBuy DECIMAL(18,2) NOT NULL CONSTRAINT DF_Categories_PerDayPriceBuy DEFAULT 30;
GO
IF COL_LENGTH('Categories', 'PerDayPriceRent') IS NULL
    ALTER TABLE Categories ADD PerDayPriceRent DECIMAL(18,2) NOT NULL CONSTRAINT DF_Categories_PerDayPriceRent DEFAULT 40;
GO

IF COL_LENGTH('Listings', 'ListingPurpose') IS NULL
    ALTER TABLE Listings ADD ListingPurpose NVARCHAR(20) NULL;
GO
IF COL_LENGTH('Listings', 'MobilePhone') IS NULL
    ALTER TABLE Listings ADD MobilePhone NVARCHAR(20) NULL;
GO
IF COL_LENGTH('Listings', 'WhatsAppPhone') IS NULL
    ALTER TABLE Listings ADD WhatsAppPhone NVARCHAR(20) NULL;
GO
IF COL_LENGTH('Listings', 'Address') IS NULL
    ALTER TABLE Listings ADD Address NVARCHAR(500) NULL;
GO
IF COL_LENGTH('Listings', 'AdDurationDays') IS NULL
    ALTER TABLE Listings ADD AdDurationDays INT NOT NULL CONSTRAINT DF_Listings_AdDurationDays DEFAULT 30;
GO
IF COL_LENGTH('Listings', 'PaymentAmount') IS NULL
    ALTER TABLE Listings ADD PaymentAmount DECIMAL(18,2) NULL;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ListingPayments')
BEGIN
    CREATE TABLE ListingPayments (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        ListingId UNIQUEIDENTIFIER NOT NULL,
        SellerId UNIQUEIDENTIFIER NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Currency NVARCHAR(10) NOT NULL DEFAULT 'LKR',
        DurationDays INT NOT NULL,
        PerDayPrice DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(30) NOT NULL DEFAULT 'Pending',
        PaymentMethod NVARCHAR(50) NULL,
        TransactionRef NVARCHAR(100) NULL,
        CardLastFour NVARCHAR(4) NULL,
        PaidAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_ListingPayments_Listing FOREIGN KEY (ListingId) REFERENCES Listings(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_ListingPayments_ListingId ON ListingPayments(ListingId);
    CREATE INDEX IX_ListingPayments_SellerId ON ListingPayments(SellerId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppSettings')
BEGIN
    CREATE TABLE AppSettings (
        [Key] NVARCHAR(100) PRIMARY KEY,
        [Value] NVARCHAR(500) NOT NULL,
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('MaxImagesPerListing', '10');
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('MinImagesPerListing', '4');
    INSERT INTO AppSettings ([Key], [Value]) VALUES ('RequireAdminApproval', 'false');
END
GO
