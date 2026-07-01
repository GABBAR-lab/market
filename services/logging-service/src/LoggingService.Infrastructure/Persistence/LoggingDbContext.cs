using LoggingService.Application.Interfaces;
using LoggingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LoggingService.Application.DTOs;

namespace LoggingService.Infrastructure.Persistence;

public class LoggingDbContext : DbContext
{
    public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

    public DbSet<ServiceLogEntry> Logs => Set<ServiceLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceLogEntry>(e =>
        {
            e.ToTable("LogEntries");
            e.HasKey(x => x.Id);
            e.Property(x => x.Level).HasMaxLength(20);
            e.Property(x => x.Service).HasMaxLength(100);
            e.Property(x => x.EventType).HasMaxLength(100);
            e.Property(x => x.Message).HasMaxLength(4000);
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.Service);
            e.HasIndex(x => x.Level);
        });
    }
}

public class LogRepository : ILogRepository
{
    private readonly LoggingDbContext _db;

    public LogRepository(LoggingDbContext db) => _db = db;

    public async Task<ServiceLogEntry> AddAsync(ServiceLogEntry entry, CancellationToken cancellationToken = default)
    {
        _db.Logs.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<(IReadOnlyList<ServiceLogEntry> Items, int Total)> SearchAsync(
        LogSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Logs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Service))
        {
            query = query.Where(l => l.Service == request.Service);
        }

        if (!string.IsNullOrWhiteSpace(request.Level))
        {
            query = query.Where(l => l.Level == request.Level);
        }

        if (!string.IsNullOrWhiteSpace(request.EventType))
        {
            query = query.Where(l => l.EventType == request.EventType);
        }

        if (request.From.HasValue)
        {
            query = query.Where(l => l.CreatedAt >= request.From.Value);
        }

        if (request.To.HasValue)
        {
            query = query.Where(l => l.CreatedAt <= request.To.Value);
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
