using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Application.Products.Abstractions;

public interface IProductCommandRepository
{
    Task<Guid> AddAsync(
        Product product,
        CancellationToken ct);

    Task<bool> UpdateAsync(
        ProductUpdate productUpdate,
        CancellationToken ct);

    Task<bool> DeleteAsync(
        Guid publicId,
        CancellationToken ct);
}
