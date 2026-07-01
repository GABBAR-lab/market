using NotificationService.Application.Interfaces;
using StackExchange.Redis;

namespace NotificationService.Infrastructure.Caching;

public class RedisUnreadCountCache : IUnreadCountCache
{
    private readonly IConnectionMultiplexer _redis;
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);

    public RedisUnreadCountCache(IConnectionMultiplexer redis) => _redis = redis;

    public async Task<int?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(Key(userId));
        return value.HasValue && int.TryParse(value.ToString(), out var count) ? count : null;
    }

    public async Task SetAsync(Guid userId, int count, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        await db.StringSetAsync(Key(userId), count, Ttl);
    }

    public async Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync(Key(userId));
    }

    private static string Key(Guid userId) => $"notifications:unread:{userId}";
}

public class NoOpUnreadCountCache : IUnreadCountCache
{
    public Task<int?> GetAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult<int?>(null);

    public Task SetAsync(Guid userId, int count, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
