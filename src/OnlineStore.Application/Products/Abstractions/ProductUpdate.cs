using OnlineStore.Domain.Products;

namespace OnlineStore.Application.Products.Abstractions;

public sealed record ProductUpdate(
    Guid PublicId,
    ProductDetails Details);
