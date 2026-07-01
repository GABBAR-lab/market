using Dapper;
using MediaService.Application.DTOs;
using MediaService.Application.Interfaces;

namespace MediaService.Infrastructure.Persistence.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public MediaRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(Guid ownerUserId, string category, string fileName, string url, string contentType, long sizeBytes)
    {
        var id = Guid.NewGuid();
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            @"INSERT INTO MediaAssets (Id, OwnerUserId, Category, FileName, Url, ContentType, SizeBytes, CreatedAt)
              VALUES (@Id, @OwnerUserId, @Category, @FileName, @Url, @ContentType, @SizeBytes, @CreatedAt)",
            new
            {
                Id = id,
                OwnerUserId = ownerUserId,
                Category = category,
                FileName = fileName,
                Url = url,
                ContentType = contentType,
                SizeBytes = sizeBytes,
                CreatedAt = DateTime.UtcNow
            });
        return id;
    }

    public async Task<MediaAssetResponse?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<MediaAssetResponse>(
            @"SELECT Id, OwnerUserId, Category, FileName, Url, ContentType, SizeBytes, CreatedAt
              FROM MediaAssets WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<IReadOnlyList<MediaAssetResponse>> GetByOwnerAsync(Guid ownerUserId, string? category = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT Id, OwnerUserId, Category, FileName, Url, ContentType, SizeBytes, CreatedAt
                    FROM MediaAssets WHERE OwnerUserId = @OwnerUserId";
        if (!string.IsNullOrWhiteSpace(category))
        {
            sql += " AND Category = @Category";
        }

        sql += " ORDER BY CreatedAt DESC";
        var rows = await connection.QueryAsync<MediaAssetResponse>(sql, new { OwnerUserId = ownerUserId, Category = category });
        return rows.ToList();
    }

    public async Task<IReadOnlyList<MediaAssetResponse>> GetAllAsync(int page = 1, int pageSize = 50)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<MediaAssetResponse>(
            @"SELECT Id, OwnerUserId, Category, FileName, Url, ContentType, SizeBytes, CreatedAt
              FROM MediaAssets
              ORDER BY CreatedAt DESC
              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
            new { Offset = (page - 1) * pageSize, PageSize = pageSize });
        return rows.ToList();
    }

    public async Task<bool> DeleteAsync(Guid id, Guid ownerUserId, bool isAdmin)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = isAdmin
            ? "DELETE FROM MediaAssets WHERE Id = @Id"
            : "DELETE FROM MediaAssets WHERE Id = @Id AND OwnerUserId = @OwnerUserId";
        var affected = await connection.ExecuteAsync(sql, new { Id = id, OwnerUserId = ownerUserId });
        return affected > 0;
    }

    public async Task<int> GetSettingIntAsync(string key, int defaultValue)
    {
        using var connection = _connectionFactory.CreateConnection();
        var value = await connection.ExecuteScalarAsync<string?>(
            "SELECT [Value] FROM MediaSettings WHERE [Key] = @Key",
            new { Key = key });
        return int.TryParse(value, out var parsed) ? parsed : defaultValue;
    }
}
