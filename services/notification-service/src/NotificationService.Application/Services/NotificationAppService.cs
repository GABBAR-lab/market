using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Services;

public class NotificationAppService : INotificationAppService
{
    private readonly INotificationRepository _repository;
    private readonly IUnreadCountCache _cache;

    public NotificationAppService(INotificationRepository repository, IUnreadCountCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var notification = new UserNotification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Body = request.Body,
            Channel = request.Channel,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(notification, cancellationToken);
        await _cache.InvalidateAsync(request.UserId, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationResponse>> GetMyNotificationsAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var items = await _repository.GetByUserAsync(userId, page, pageSize, cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync(userId, cancellationToken);
        if (cached.HasValue)
        {
            return cached.Value;
        }

        var count = await _repository.GetUnreadCountAsync(userId, cancellationToken);
        await _cache.SetAsync(userId, count, cancellationToken);
        return count;
    }

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null || notification.UserId != userId)
        {
            return;
        }

        if (!notification.IsRead)
        {
            await _repository.MarkAsReadAsync(notificationId, cancellationToken);
            await _cache.InvalidateAsync(userId, cancellationToken);
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByUserAsync(userId, 1, 500, cancellationToken);
        foreach (var item in items.Where(i => !i.IsRead))
        {
            await _repository.MarkAsReadAsync(item.Id, cancellationToken);
        }

        await _cache.SetAsync(userId, 0, cancellationToken);
    }

    private static NotificationResponse Map(UserNotification n) => new(
        n.Id, n.Title, n.Body, n.Channel, n.ReferenceType, n.ReferenceId, n.IsRead, n.CreatedAt);
}
