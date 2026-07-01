using LoggingService.Application.Interfaces;
using LoggingService.Infrastructure.Messaging;
using LoggingService.Infrastructure.Persistence;
using MarketPlace.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoggingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LoggingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILogRepository, LogRepository>();
        services.AddRabbitMqMessaging(configuration);
        services.AddScoped<IIntegrationEventHandler, LogEntryEventHandler>();
        services.AddScoped<IIntegrationEventHandler, UniversalEventLogHandler>();
        services.AddHostedService<LoggingConsumerHostedService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
