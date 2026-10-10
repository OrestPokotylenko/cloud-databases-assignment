using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.CreateProduct;
using OnlineStore.Application.Products.DeleteProduct;
using OnlineStore.Application.Products.GetProductById;
using OnlineStore.Application.Products.GetProducts;
using OnlineStore.Application.Products.UpdateProduct;

namespace OnlineStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRequestSender, RequestSender>();

        services.AddScoped<IHandler<CreateProductCommand, CreateProductResult>, CreateProductHandler>();

        services.AddScoped<IHandler<UpdateProductCommand, UpdateProductResult>, UpdateProductHandler>();

        services.AddScoped<IHandler<DeleteProductCommand, DeleteProductResult>, DeleteProductHandler>();

        services.AddScoped<IHandler<GetProductByIdQuery, GetProductByIdResult?>, GetProductByIdHandler>();

        services.AddScoped<IHandler<GetProductsQuery, GetProductsResult>, GetProductsHandler>();

        return services;
    }
}
