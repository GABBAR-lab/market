using MarketPlace.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Caching;
using NotificationService.Infrastructure.Messaging;
using NotificationService.Infrastructure.Persistence;
using StackExchange.Redis;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<INotificationRepository, NotificationRepository>();

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
            services.AddSingleton<IUnreadCountCache, RedisUnreadCountCache>();
        }
        else
        {
            services.AddSingleton<IUnreadCountCache, NoOpUnreadCountCache>();
        }

        services.AddRabbitMqMessaging(configuration);
        services.AddScoped<IIntegrationEventHandler, NotificationSendEventHandler>();
        services.AddScoped<IIntegrationEventHandler, PaymentCompletedEventHandler>();
        services.AddHostedService<NotificationConsumerHostedService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
