using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationRepository
{
    Task<UserNotification> AddAsync(UserNotification notification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserNotification>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IUnreadCountCache
{
    Task<int?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SetAsync(Guid userId, int count, CancellationToken cancellationToken = default);
    Task InvalidateAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface INotificationAppService
{
    Task CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationResponse>> GetMyNotificationsAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
}
