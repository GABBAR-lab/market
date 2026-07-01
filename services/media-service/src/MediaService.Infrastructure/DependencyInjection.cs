using MediaService.Application.Interfaces;
using MediaService.Infrastructure.Persistence;
using MediaService.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<DatabaseInitializer>();
        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }
}
