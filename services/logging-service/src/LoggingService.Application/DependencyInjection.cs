using Microsoft.Extensions.DependencyInjection;
using LoggingService.Application.Interfaces;
using LoggingService.Application.Services;

namespace LoggingService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILogAppService, LogAppService>();
        return services;
    }
}
