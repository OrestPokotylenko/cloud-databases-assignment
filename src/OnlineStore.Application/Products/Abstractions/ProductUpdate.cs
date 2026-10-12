using OnlineStore.Domain.Products;

namespace OnlineStore.Application.Products.Abstractions;

public record ProductUpdate(
    Guid PublicId,
    ProductDetails Details);
