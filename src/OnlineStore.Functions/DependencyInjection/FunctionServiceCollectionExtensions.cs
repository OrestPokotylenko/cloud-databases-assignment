using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;

using OnlineStore.Application.DependencyInjection;
using OnlineStore.ExternalServices.DependencyInjection;
using OnlineStore.Functions.Messaging;
using OnlineStore.Persistence.DependencyInjection;

namespace OnlineStore.Functions.DependencyInjection;

public static class FunctionServiceCollectionExtensions
{
    public static IServiceCollection AddFunctionServices(
        this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddQueueServices();
        services.AddPersistenceServices();
        services.AddScoped<QueueCommandProcessor>();

        services.AddOpenTelemetry()
            .UseFunctionsWorkerDefaults();

        return services;
    }
}
