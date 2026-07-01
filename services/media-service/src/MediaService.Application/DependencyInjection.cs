using MediaService.Application.Interfaces;
using MediaService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediaAppService, MediaAppService>();
        return services;
    }
}
