namespace OnlineStore.Application.Products.Abstractions;

public record ProductPageData(
    IReadOnlyList<ProductData> Products,
    int TotalCount);
