using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid PublicId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity) : ICommand<UpdateProductResult>;
