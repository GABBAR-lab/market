using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;

namespace PaymentService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPaymentAppService, PaymentAppService>();
        return services;
    }
}
