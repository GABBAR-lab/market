using ProfileService.Application.Interfaces;
using ProfileService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ProfileService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, Services.ProfileService>();

        return services;
    }
}
