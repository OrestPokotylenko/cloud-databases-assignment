using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Application.UnitTests.Products;

internal class ProductCommandRepositoryStub : IProductCommandRepository
{
    public Product? AddedProduct { get; private set; }
    public ProductUpdate? ReceivedUpdate { get; private set; }
    public Guid? DeletedPublicId { get; private set; }
    public CancellationToken ReceivedCancellationToken { get; private set; }
    public Guid AddedPublicId { get; set; } = Guid.NewGuid();
    public bool UpdateResult { get; set; } = true;
    public bool DeleteResult { get; set; } = true;

    public Task<Guid> AddAsync(
        Product product,
        CancellationToken ct)
    {
        AddedProduct = product;
        ReceivedCancellationToken = ct;
        return Task.FromResult(AddedPublicId);
    }

    public Task<bool> UpdateAsync(
        ProductUpdate productUpdate,
        CancellationToken ct)
    {
        ReceivedUpdate = productUpdate;
        ReceivedCancellationToken = ct;
        return Task.FromResult(UpdateResult);
    }

    public Task<bool> DeleteAsync(
        Guid publicId,
        CancellationToken ct)
    {
        DeletedPublicId = publicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(DeleteResult);
    }
}
