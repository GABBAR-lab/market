using LoggingService.Application.DTOs;
using LoggingService.Application.Interfaces;
using LoggingService.Domain.Entities;

namespace LoggingService.Application.Services;

public class LogAppService : ILogAppService
{
    private readonly ILogRepository _repository;

    public LogAppService(ILogRepository repository) => _repository = repository;

    public Task WriteAsync(ServiceLogEntry entry, CancellationToken cancellationToken = default) =>
        _repository.AddAsync(entry, cancellationToken);

    public async Task<(IReadOnlyList<LogEntryResponse> Items, int Total)> SearchAsync(
        LogSearchRequest request, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repository.SearchAsync(request, cancellationToken);
        return (items.Select(Map).ToList(), total);
    }

    private static LogEntryResponse Map(ServiceLogEntry e) => new(
        e.Id, e.Level, e.Service, e.EventType, e.Message, e.CorrelationId, e.UserId, e.Exception, e.CreatedAt);
}
