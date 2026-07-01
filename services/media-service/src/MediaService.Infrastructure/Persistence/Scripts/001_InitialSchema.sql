IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SchemaVersions')
BEGIN
    CREATE TABLE SchemaVersions (
        ScriptName NVARCHAR(200) NOT NULL PRIMARY KEY,
        AppliedAt DATETIME2 NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MediaAssets')
BEGIN
    CREATE TABLE MediaAssets (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        OwnerUserId UNIQUEIDENTIFIER NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        FileName NVARCHAR(260) NOT NULL,
        Url NVARCHAR(512) NOT NULL,
        ContentType NVARCHAR(100) NOT NULL,
        SizeBytes BIGINT NOT NULL,
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_MediaAssets_OwnerUserId ON MediaAssets(OwnerUserId);
    CREATE INDEX IX_MediaAssets_Category ON MediaAssets(Category);
    CREATE INDEX IX_MediaAssets_CreatedAt ON MediaAssets(CreatedAt DESC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MediaSettings')
BEGIN
    CREATE TABLE MediaSettings (
        [Key] NVARCHAR(100) NOT NULL PRIMARY KEY,
        [Value] NVARCHAR(500) NOT NULL
    );

    INSERT INTO MediaSettings ([Key], [Value]) VALUES
        ('MaxImagesPerListing', '10'),
        ('MaxAvatarUploads', '1');
END
GO
