using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.DeleteProduct;

public record DeleteProductCommand(Guid PublicId)
    : ICommand<DeleteProductResult>;
