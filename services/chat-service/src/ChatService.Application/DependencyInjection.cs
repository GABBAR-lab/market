using Microsoft.Extensions.DependencyInjection;
using ChatService.Application.Interfaces;
using ChatService.Application.Services;

namespace ChatService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IChatAppService, ChatAppService>();
        return services;
    }
}
