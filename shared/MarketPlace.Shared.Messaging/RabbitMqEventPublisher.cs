using System.Text;
using System.Text.Json;
using MarketPlace.Shared.Contracts.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MarketPlace.Shared.Messaging;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "marketplace.events";
}

public interface IEventPublisher
{
    Task PublishAsync<T>(string eventType, T payload, CancellationToken cancellationToken = default);
}

public sealed class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEventPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string eventType, T payload, CancellationToken cancellationToken = default)
    {
        await EnsureChannelAsync(cancellationToken);

        var integrationEvent = new IntegrationEvent(
            eventType,
            Guid.NewGuid(),
            DateTime.UtcNow,
            JsonSerializer.Serialize(payload));

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent));

        var props = new BasicProperties { ContentType = "application/json", DeliveryMode = DeliveryModes.Persistent };
        await _channel!.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: eventType,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published event {EventType} {EventId}", eventType, integrationEvent.EventId);
    }

    private async Task EnsureChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_channel is { IsOpen: true })
            {
                return;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _lock.Dispose();
    }
}
