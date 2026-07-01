using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<UserNotification> Notifications => Set<UserNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserNotification>(e =>
        {
            e.ToTable("Notifications");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200);
            e.Property(x => x.Body).HasMaxLength(2000);
            e.Property(x => x.Channel).HasMaxLength(50);
            e.Property(x => x.ReferenceType).HasMaxLength(100);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.UserId, x.IsRead });
        });
    }
}

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _db;

    public NotificationRepository(NotificationDbContext db) => _db = db;

    public async Task<UserNotification> AddAsync(UserNotification notification, CancellationToken cancellationToken = default)
    {
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(cancellationToken);
        return notification;
    }

    public async Task<IReadOnlyList<UserNotification>> GetByUserAsync(
        Guid userId, int page, int pageSize, CancellationToken cancellationToken = default) =>
        await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _db.Notifications.FindAsync([id], cancellationToken);

    public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _db.Notifications.FindAsync([id], cancellationToken);
        if (notification is null)
        {
            return;
        }

        notification.IsRead = true;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
}
