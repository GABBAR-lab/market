using ChatService.Application.Interfaces;
using ChatService.Infrastructure.Persistence;
using MarketPlace.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddRabbitMqMessaging(configuration);

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
