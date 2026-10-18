using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.ConfirmProductImageUpload;
using OnlineStore.Application.Products.CreateProduct;
using OnlineStore.Application.Products.DeleteProduct;
using OnlineStore.Application.Products.DeleteProductImage;
using OnlineStore.Application.Products.GetProductById;
using OnlineStore.Application.Products.GetProductImages;
using OnlineStore.Application.Products.GetProducts;
using OnlineStore.Application.Products.RequestProductImageUpload;
using OnlineStore.Application.Products.SetPrimaryProductImage;
using OnlineStore.Application.Products.UpdateProduct;

namespace OnlineStore.Application.DependencyInjection;

internal static class ProductServiceCollectionExtensions
{
    public static IServiceCollection AddProductServices(
        this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateProductCommand, CreateProductResult>, CreateProductHandler>();
        services.AddScoped<IHandler<UpdateProductCommand, UpdateProductResult>, UpdateProductHandler>();
        services.AddScoped<IHandler<DeleteProductCommand, DeleteProductResult>, DeleteProductHandler>();
        services.AddScoped<IHandler<GetProductByIdQuery, GetProductByIdResult?>, GetProductByIdHandler>();
        services.AddScoped<IHandler<GetProductsQuery, GetProductsResult>, GetProductsHandler>();
        services.AddScoped<IHandler<RequestProductImageUploadCommand, RequestProductImageUploadResult>, RequestProductImageUploadHandler>();
        services.AddScoped<IHandler<ConfirmProductImageUploadCommand, ConfirmProductImageUploadResult>, ConfirmProductImageUploadHandler>();
        services.AddScoped<IHandler<GetProductImagesQuery, GetProductImagesResult>, GetProductImagesHandler>();
        services.AddScoped<IHandler<DeleteProductImageCommand, DeleteProductImageResult>, DeleteProductImageHandler>();
        services.AddScoped<IHandler<SetPrimaryProductImageCommand, SetPrimaryProductImageResult>, SetPrimaryProductImageHandler>();

        return services;
    }
}
