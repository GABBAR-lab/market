using Dapper;
using ListingService.Application.Interfaces;
using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Infrastructure.Persistence.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public LocationRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Location?> GetByIdAsync(Guid id, bool includeChildren = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        var row = await connection.QuerySingleOrDefaultAsync<LocationRow>(
            "SELECT * FROM Locations WHERE Id = @Id", new { Id = id });

        if (row is null) return null;

        var location = MapLocation(row);

        if (includeChildren)
        {
            var children = await GetChildrenInternalAsync(connection, id);
            location.LoadChildren(children);
        }

        return location;
    }

    public async Task<Location?> GetBySlugAsync(string slug)
    {
        using var connection = _connectionFactory.CreateConnection();

        var row = await connection.QuerySingleOrDefaultAsync<LocationRow>(
            "SELECT * FROM Locations WHERE Slug = @Slug", new { Slug = slug.ToLowerInvariant() });

        return row is null ? null : MapLocation(row);
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(bool activeOnly = true)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = activeOnly
            ? "SELECT * FROM Locations WHERE IsActive = 1 ORDER BY SortOrder"
            : "SELECT * FROM Locations ORDER BY SortOrder";

        var rows = await connection.QueryAsync<LocationRow>(sql);
        return rows.Select(MapLocation).ToList();
    }

    public async Task<IReadOnlyList<Location>> GetByTypeAsync(LocationType type, bool activeOnly = true)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = activeOnly
            ? "SELECT * FROM Locations WHERE Type = @Type AND IsActive = 1 ORDER BY SortOrder"
            : "SELECT * FROM Locations WHERE Type = @Type ORDER BY SortOrder";

        var rows = await connection.QueryAsync<LocationRow>(sql, new { Type = (int)type });
        return rows.Select(MapLocation).ToList();
    }

    public async Task<IReadOnlyList<Location>> GetChildrenAsync(Guid parentLocationId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await GetChildrenInternalAsync(connection, parentLocationId);
    }

    public async Task AddAsync(Location location)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(@"
            INSERT INTO Locations (Id, Name, Slug, Type, ParentLocationId, SortOrder, IsActive, CreatedAt, UpdatedAt)
            VALUES (@Id, @Name, @Slug, @Type, @ParentLocationId, @SortOrder, @IsActive, @CreatedAt, @UpdatedAt)",
            new
            {
                location.Id,
                location.Name,
                location.Slug,
                Type = (int)location.Type,
                location.ParentLocationId,
                location.SortOrder,
                location.IsActive,
                location.CreatedAt,
                location.UpdatedAt
            });
    }

    public async Task UpdateAsync(Location location)
    {
        using var connection = _connectionFactory.CreateConnection();
        location.MarkAsUpdated();

        await connection.ExecuteAsync(@"
            UPDATE Locations SET
                Name = @Name, Slug = @Slug, SortOrder = @SortOrder, IsActive = @IsActive, UpdatedAt = @UpdatedAt
            WHERE Id = @Id",
            new
            {
                location.Id,
                location.Name,
                location.Slug,
                location.SortOrder,
                location.IsActive,
                location.UpdatedAt
            });
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? parentLocationId, Guid? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = excludeId.HasValue
            ? @"SELECT COUNT(1) FROM Locations WHERE Slug = @Slug
                AND ((@ParentLocationId IS NULL AND ParentLocationId IS NULL) OR ParentLocationId = @ParentLocationId)
                AND Id <> @ExcludeId"
            : @"SELECT COUNT(1) FROM Locations WHERE Slug = @Slug
                AND ((@ParentLocationId IS NULL AND ParentLocationId IS NULL) OR ParentLocationId = @ParentLocationId)";

        return await connection.ExecuteScalarAsync<int>(sql,
            new { Slug = slug.ToLowerInvariant(), ParentLocationId = parentLocationId, ExcludeId = excludeId }) > 0;
    }

    private static async Task<IReadOnlyList<Location>> GetChildrenInternalAsync(System.Data.IDbConnection connection, Guid parentLocationId)
    {
        var rows = await connection.QueryAsync<LocationRow>(
            "SELECT * FROM Locations WHERE ParentLocationId = @ParentLocationId AND IsActive = 1 ORDER BY SortOrder",
            new { ParentLocationId = parentLocationId });

        return rows.Select(MapLocation).ToList();
    }

    private static Location MapLocation(LocationRow row) =>
        Location.Restore(row.Id, row.Name, row.Slug, (LocationType)row.Type,
            row.ParentLocationId, row.SortOrder, row.IsActive, row.CreatedAt, row.UpdatedAt);

    private class LocationRow
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
