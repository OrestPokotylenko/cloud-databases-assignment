using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Abstractions;

namespace OnlineStore.Application.Products.GetProducts;

public class GetProductsHandler(
    IProductQueryRepository productRepository)
    : IHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> HandleAsync(
        GetProductsQuery request,
        CancellationToken ct)
    {
        if (request.PageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.PageNumber),
                "The page number must be at least 1.");
        }

        if (request.PageSize is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.PageSize),
                "The page size must be between 1 and 100.");
        }

        var skip = (long)(request.PageNumber - 1) * request.PageSize;
        if (skip > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.PageNumber),
                "The requested page is outside the supported range.");
        }

        var productPage = await productRepository.GetPageAsync(
            (int)skip,
            request.PageSize,
            ct);
        var products = productPage.Products
            .Select(product => new ProductListItem(
                product.PublicId,
                product.CreatedAt,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity))
            .ToArray();
        var totalPages = productPage.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(
                productPage.TotalCount / (double)request.PageSize);

        return new GetProductsResult(
            request.PageNumber,
            request.PageSize,
            productPage.TotalCount,
            totalPages,
            products);
    }
}
