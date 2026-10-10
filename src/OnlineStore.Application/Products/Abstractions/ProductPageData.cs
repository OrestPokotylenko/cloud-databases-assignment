namespace OnlineStore.Application.Products.Abstractions;

public sealed record ProductPageData(
    IReadOnlyList<ProductData> Products,
    int TotalCount);
