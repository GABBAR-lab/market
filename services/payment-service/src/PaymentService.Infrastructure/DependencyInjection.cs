using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;
using PaymentService.Infrastructure.Http;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddHttpClient<IListingServiceClient, ListingServiceClient>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
