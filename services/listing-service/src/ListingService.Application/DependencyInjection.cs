using ListingService.Application.Interfaces;
using ListingService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ListingService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IListingService, Services.ListingService>();
        services.AddScoped<ILocationService, LocationService>();
        return services;
    }
}
