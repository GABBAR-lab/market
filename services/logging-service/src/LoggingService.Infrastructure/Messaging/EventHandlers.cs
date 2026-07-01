using System.Text.Json;
using MarketPlace.Shared.Contracts.Events;
using MarketPlace.Shared.Messaging;
using LoggingService.Application.Interfaces;
using LoggingService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoggingService.Infrastructure.Messaging;

public class LogEntryEventHandler : IIntegrationEventHandler
{
    private readonly ILogAppService _logs;

    public LogEntryEventHandler(ILogAppService logs) => _logs = logs;

    public string EventType => EventTypes.LogEntry;

    public async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Deserialize<LogEntryPayload>(integrationEvent.PayloadJson);
        if (payload is null)
        {
            return;
        }

        await _logs.WriteAsync(new ServiceLogEntry
        {
            Id = Guid.NewGuid(),
            Level = payload.Level,
            Service = payload.Service,
            EventType = integrationEvent.EventType,
            Message = payload.Message,
            CorrelationId = payload.CorrelationId,
            UserId = payload.UserId,
            Exception = payload.Exception,
            PayloadJson = integrationEvent.PayloadJson,
            CreatedAt = integrationEvent.OccurredAt
        }, cancellationToken);
    }
}

public class UniversalEventLogHandler : IIntegrationEventHandler
{
    private readonly ILogAppService _logs;

    public UniversalEventLogHandler(ILogAppService logs) => _logs = logs;

    public string EventType => "*";

    public async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (integrationEvent.EventType == EventTypes.LogEntry)
        {
            return;
        }

        await _logs.WriteAsync(new ServiceLogEntry
        {
            Id = Guid.NewGuid(),
            Level = "Information",
            Service = "EventBus",
            EventType = integrationEvent.EventType,
            Message = $"Integration event received: {integrationEvent.EventType}",
            CorrelationId = integrationEvent.EventId.ToString(),
            PayloadJson = integrationEvent.PayloadJson,
            CreatedAt = integrationEvent.OccurredAt
        }, cancellationToken);
    }
}

public class LoggingConsumerHostedService : RabbitMqConsumerHostedService
{
    public LoggingConsumerHostedService(
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<LoggingConsumerHostedService> logger)
        : base(options, scopeFactory, logger, "logging-service",
            EventTypes.LogEntry,
            EventTypes.PaymentCompleted,
            EventTypes.PaymentFailed,
            EventTypes.ListingCreated,
            EventTypes.ListingActivated,
            EventTypes.NotificationSend,
            EventTypes.ChatMessageSent)
    {
    }
}
