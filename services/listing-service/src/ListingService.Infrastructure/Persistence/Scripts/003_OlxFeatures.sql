IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ListingReports')
BEGIN
    CREATE TABLE ListingReports (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ListingId UNIQUEIDENTIFIER NOT NULL,
        ReporterUserId UNIQUEIDENTIFIER NULL,
        Reason NVARCHAR(100) NOT NULL,
        Comment NVARCHAR(1000) NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT FK_ListingReports_Listing FOREIGN KEY (ListingId) REFERENCES Listings(Id)
    );
    CREATE INDEX IX_ListingReports_ListingId ON ListingReports(ListingId);
    CREATE INDEX IX_ListingReports_Status ON ListingReports(Status);
END
GO
