using System.Text;
using System.Text.Json;
using MarketPlace.Shared.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MarketPlace.Shared.Messaging;

public interface IIntegrationEventHandler
{
    string EventType { get; }
    Task HandleAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken);
}

public abstract class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;
    private readonly string _queueName;
    private readonly string[] _routingKeys;

    protected RabbitMqConsumerHostedService(
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger logger,
        string queueName,
        params string[] routingKeys)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _queueName = queueName;
        _routingKeys = routingKeys;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunConsumerAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "RabbitMQ consumer disconnected. Retrying in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task RunConsumerAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Topic, true, false, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(_queueName, true, false, false, cancellationToken: stoppingToken);

        foreach (var key in _routingKeys)
        {
            await channel.QueueBindAsync(_queueName, _options.ExchangeName, key, cancellationToken: stoppingToken);
        }

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var integrationEvent = JsonSerializer.Deserialize<IntegrationEvent>(json);
                if (integrationEvent is null)
                {
                    await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var handlers = scope.ServiceProvider.GetServices<IIntegrationEventHandler>()
                    .Where(h => h.EventType == integrationEvent.EventType || h.EventType == "*");

                foreach (var handler in handlers)
                {
                    await handler.HandleAsync(integrationEvent, stoppingToken);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                await channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(_queueName, false, consumer, stoppingToken);
        _logger.LogInformation("RabbitMQ consumer listening on {Queue}", _queueName);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        return services;
    }
}
