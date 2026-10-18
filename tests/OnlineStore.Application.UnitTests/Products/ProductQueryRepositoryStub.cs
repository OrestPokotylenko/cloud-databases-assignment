using OnlineStore.Application.Products.Abstractions;

namespace OnlineStore.Application.UnitTests.Products;

internal class ProductQueryRepositoryStub : IProductQueryRepository
{
    public bool ProductExists { get; set; } = true;
    public ProductData? Product { get; set; }
    public ProductPageData Page { get; set; } = new([], 0);
    public Guid? RequestedPublicId { get; private set; }
    public int? RequestedSkip { get; private set; }
    public int? RequestedTake { get; private set; }
    public CancellationToken ReceivedCancellationToken { get; private set; }

    public Task<bool> ExistsAsync(
        Guid publicId,
        CancellationToken ct)
    {
        RequestedPublicId = publicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(ProductExists);
    }

    public Task<ProductData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct)
    {
        RequestedPublicId = publicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(Product);
    }

    public Task<ProductPageData> GetPageAsync(
        int skip,
        int take,
        CancellationToken ct)
    {
        RequestedSkip = skip;
        RequestedTake = take;
        ReceivedCancellationToken = ct;
        return Task.FromResult(Page);
    }
}
