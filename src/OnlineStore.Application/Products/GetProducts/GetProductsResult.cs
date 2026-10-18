namespace OnlineStore.Application.Products.GetProducts;

public record GetProductsResult(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    IReadOnlyList<ProductListItem> Products);
