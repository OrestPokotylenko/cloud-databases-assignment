using System.Text.Json;
using System.Text.Json.Serialization;

using Asp.Versioning;

using OnlineStore.API.ExceptionHandling;
using OnlineStore.Application.DependencyInjection;
using OnlineStore.Domain.Orders;
using OnlineStore.ExternalServices.DependencyInjection;
using OnlineStore.Persistence.DependencyInjection;

namespace OnlineStore.API.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter<OrderStatus>(
                        JsonNamingPolicy.CamelCase,
                        false)))
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory =
                    InvalidModelStateResponseFactory.Create;
                options.SuppressMapClientErrors = true;
            });

        services.AddOpenApi();
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();

        services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ReportApiVersions = true;
            })
            .AddMvc();

        services.AddApplicationServices();
        services.AddQueueServices();
        services.AddProductImageStorageServices();
        services.AddProductReviewStorageServices();
        services.AddPersistenceServices();

        return services;
    }
}
