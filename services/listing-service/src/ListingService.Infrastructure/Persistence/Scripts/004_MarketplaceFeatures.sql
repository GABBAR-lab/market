IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ListingFavorites')
BEGIN
    CREATE TABLE ListingFavorites (
        UserId UNIQUEIDENTIFIER NOT NULL,
        ListingId UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT PK_ListingFavorites PRIMARY KEY (UserId, ListingId),
        CONSTRAINT FK_ListingFavorites_Listing FOREIGN KEY (ListingId) REFERENCES Listings(Id)
    );
    CREATE INDEX IX_ListingFavorites_UserId ON ListingFavorites(UserId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ListingInquiries')
BEGIN
    CREATE TABLE ListingInquiries (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ListingId UNIQUEIDENTIFIER NOT NULL,
        BuyerUserId UNIQUEIDENTIFIER NULL,
        BuyerName NVARCHAR(200) NOT NULL,
        BuyerPhone NVARCHAR(30) NOT NULL,
        Message NVARCHAR(2000) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'New',
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT FK_ListingInquiries_Listing FOREIGN KEY (ListingId) REFERENCES Listings(Id)
    );
    CREATE INDEX IX_ListingInquiries_ListingId ON ListingInquiries(ListingId);
    CREATE INDEX IX_ListingInquiries_BuyerUserId ON ListingInquiries(BuyerUserId);
END
GO
