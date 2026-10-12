using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IRequestSender, RequestSender>();
        services.AddSingleton(TimeProvider.System);

        services.AddOrderServices();
        services.AddProductServices();

        return services;
    }
}
