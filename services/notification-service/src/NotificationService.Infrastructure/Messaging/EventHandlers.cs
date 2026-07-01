using System.Text.Json;
using MarketPlace.Shared.Contracts.Events;
using MarketPlace.Shared.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Messaging;

public class NotificationSendEventHandler : IIntegrationEventHandler
{
    private readonly INotificationAppService _notifications;

    public NotificationSendEventHandler(INotificationAppService notifications) => _notifications = notifications;

    public string EventType => EventTypes.NotificationSend;

    public async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Deserialize<NotificationSendPayload>(integrationEvent.PayloadJson);
        if (payload is null)
        {
            return;
        }

        await _notifications.CreateAsync(new CreateNotificationRequest(
            payload.UserId, payload.Title, payload.Body, payload.Channel,
            payload.ReferenceType, payload.ReferenceId), cancellationToken);
    }
}

public class PaymentCompletedEventHandler : IIntegrationEventHandler
{
    private readonly INotificationAppService _notifications;

    public PaymentCompletedEventHandler(INotificationAppService notifications) => _notifications = notifications;

    public string EventType => EventTypes.PaymentCompleted;

    public async Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Deserialize<PaymentCompletedPayload>(integrationEvent.PayloadJson);
        if (payload is null)
        {
            return;
        }

        await _notifications.CreateAsync(new CreateNotificationRequest(
            payload.SellerId,
            "Payment completed",
            $"Your payment of Rs {payload.Amount:N0} for listing was successful. Status: {payload.ListingStatus}.",
            "InApp",
            "Payment",
            payload.PaymentId), cancellationToken);
    }
}

public class NotificationConsumerHostedService : RabbitMqConsumerHostedService
{
    public NotificationConsumerHostedService(
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationConsumerHostedService> logger)
        : base(options, scopeFactory, logger, "notification-service", EventTypes.NotificationSend, EventTypes.PaymentCompleted)
    {
    }
}
