using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.GetProducts;

public class GetProductsHandler(
    IProductQueryRepository productRepository,
    IProductImageStorage imageStorage)
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
            .Select(product =>
            {
                var primaryImage = product.Images.SingleOrDefault(
                    image => image.IsPrimary);

                return new ProductListItem(
                    product.PublicId,
                    product.CreatedAt,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.StockQuantity,
                    primaryImage is null
                        ? null
                        : imageStorage.GetPublicUrl(primaryImage.BlobName));
            })
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
