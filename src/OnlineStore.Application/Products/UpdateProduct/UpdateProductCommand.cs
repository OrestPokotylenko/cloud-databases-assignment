using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid PublicId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity) : ICommand<UpdateProductResult>;
