namespace OnlineStore.Application.Products.GetProductById;

public sealed record GetProductByIdResult(
    Guid PublicId,
    DateTimeOffset CreatedAt,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);
