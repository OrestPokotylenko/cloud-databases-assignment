namespace OnlineStore.Application.Products.GetProducts;

public sealed record ProductListItem(
    Guid PublicId,
    DateTimeOffset CreatedAt,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);
