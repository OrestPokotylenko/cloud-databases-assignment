using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid PublicId)
    : ICommand<DeleteProductResult>;
