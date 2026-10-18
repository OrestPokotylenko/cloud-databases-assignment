using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.ProductReviews.CreateProductReview;
using OnlineStore.Application.ProductReviews.DeleteProductReview;
using OnlineStore.Application.ProductReviews.GetProductReviews;

namespace OnlineStore.Application.DependencyInjection;

internal static class ProductReviewServiceCollectionExtensions
{
    public static IServiceCollection AddProductReviewServices(
        this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateProductReviewCommand, CreateProductReviewResult>, CreateProductReviewHandler>();
        services.AddScoped<IHandler<GetProductReviewsQuery, GetProductReviewsResult>, GetProductReviewsHandler>();
        services.AddScoped<IHandler<DeleteProductReviewCommand, DeleteProductReviewResult>, DeleteProductReviewHandler>();

        return services;
    }
}
