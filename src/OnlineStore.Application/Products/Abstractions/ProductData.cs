namespace OnlineStore.Application.Products.Abstractions;

public sealed record ProductData(
    Guid PublicId,
    DateTimeOffset CreatedAt,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);
