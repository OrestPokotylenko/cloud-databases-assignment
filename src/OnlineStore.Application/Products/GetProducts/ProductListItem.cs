namespace OnlineStore.Application.Products.GetProducts;

public record ProductListItem(
    Guid PublicId,
    DateTimeOffset CreatedAt,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? PrimaryImageUrl);
