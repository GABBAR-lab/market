using LoggingService.Application.DTOs;
using LoggingService.Domain.Entities;

namespace LoggingService.Application.Interfaces;

public interface ILogRepository
{
    Task<ServiceLogEntry> AddAsync(ServiceLogEntry entry, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<ServiceLogEntry> Items, int Total)> SearchAsync(LogSearchRequest request, CancellationToken cancellationToken = default);
}

public interface ILogAppService
{
    Task WriteAsync(ServiceLogEntry entry, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<LogEntryResponse> Items, int Total)> SearchAsync(LogSearchRequest request, CancellationToken cancellationToken = default);
}
