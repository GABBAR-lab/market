using Dapper;
using ListingService.Application.Interfaces;

namespace ListingService.Infrastructure.Persistence.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public FavoriteRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Guid>> GetListingIdsByUserAsync(Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var ids = await connection.QueryAsync<Guid>(
            "SELECT ListingId FROM ListingFavorites WHERE UserId = @UserId ORDER BY CreatedAt DESC",
            new { UserId = userId });
        return ids.ToList();
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid listingId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM ListingFavorites WHERE UserId = @UserId AND ListingId = @ListingId",
            new { UserId = userId, ListingId = listingId }) > 0;
    }

    public async Task AddAsync(Guid userId, Guid listingId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            @"IF NOT EXISTS (SELECT 1 FROM ListingFavorites WHERE UserId = @UserId AND ListingId = @ListingId)
              INSERT INTO ListingFavorites (UserId, ListingId, CreatedAt) VALUES (@UserId, @ListingId, @CreatedAt)",
            new { UserId = userId, ListingId = listingId, CreatedAt = DateTime.UtcNow });
    }

    public async Task RemoveAsync(Guid userId, Guid listingId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "DELETE FROM ListingFavorites WHERE UserId = @UserId AND ListingId = @ListingId",
            new { UserId = userId, ListingId = listingId });
    }
}
