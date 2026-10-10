using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity) : ICommand<CreateProductResult>;
