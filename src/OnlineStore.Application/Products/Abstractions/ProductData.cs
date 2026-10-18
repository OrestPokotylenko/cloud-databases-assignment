using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.Abstractions;

public record ProductData(
    Guid PublicId,
    DateTimeOffset CreatedAt,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    IReadOnlyList<ProductImageData> Images);
