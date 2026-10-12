using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Application.Products.CreateProduct;

public class CreateProductHandler(
    IProductCommandRepository productRepository)
    : IHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> HandleAsync(
        CreateProductCommand request,
        CancellationToken ct)
    {
        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity);

        var publicId = await productRepository.AddAsync(product, ct);

        return new CreateProductResult(publicId);
    }
}
