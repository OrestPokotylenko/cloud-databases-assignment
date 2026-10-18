using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.GetProductById;

public record GetProductByIdResult(
    Guid PublicId,
    DateTimeOffset CreatedAt,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    IReadOnlyList<ProductImageResult> Images);
