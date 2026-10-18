using Azure;
using Azure.Data.Tables;

using OnlineStore.Application.ProductReviews.Abstractions;
using OnlineStore.Domain.ProductReviews;

namespace OnlineStore.ExternalServices.TableStorage;

internal class AzureTableProductReviewRepository(
    TableClient tableClient)
    : IProductReviewRepository
{
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private volatile bool initialized;

    public async Task AddAsync(
        ProductReview review,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(review);

        await EnsureTableExistsAsync(ct);
        await tableClient.AddEntityAsync(Map(review), ct);
    }

    public async Task<ProductReviewPage> GetByProductAsync(
        Guid productPublicId,
        int pageSize,
        string? continuationToken,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(productPublicId, Guid.Empty);

        await EnsureTableExistsAsync(ct);

        var partitionKey = productPublicId.ToString();
        var filter = TableClient.CreateQueryFilter(
            $"PartitionKey eq {partitionKey}");

        try
        {
            var pages = tableClient
                .QueryAsync<ProductReviewTableEntity>(
                    filter,
                    pageSize,
                    cancellationToken: ct)
                .AsPages(continuationToken, pageSize);

            await foreach (var page in pages.WithCancellation(ct))
            {
                return new ProductReviewPage(
                    page.Values.Select(Map).ToArray(),
                    page.ContinuationToken);
            }

            return new ProductReviewPage([], null);
        }
        catch (RequestFailedException exception)
            when (exception.Status == 400 &&
                  !string.IsNullOrWhiteSpace(continuationToken))
        {
            throw new ArgumentException(
                "The continuation token is invalid.",
                nameof(continuationToken),
                exception);
        }
    }

    public async Task<bool> DeleteAsync(
        Guid productPublicId,
        Guid reviewPublicId,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(productPublicId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(reviewPublicId, Guid.Empty);

        await EnsureTableExistsAsync(ct);

        try
        {
            await tableClient.DeleteEntityAsync(
                productPublicId.ToString(),
                reviewPublicId.ToString(),
                ETag.All,
                ct);
            return true;
        }
        catch (RequestFailedException exception) when (exception.Status == 404)
        {
            return false;
        }
    }

    private async Task EnsureTableExistsAsync(CancellationToken ct)
    {
        if (initialized)
        {
            return;
        }

        await initializationLock.WaitAsync(ct);
        try
        {
            if (!initialized)
            {
                await tableClient.CreateIfNotExistsAsync(ct);
                initialized = true;
            }
        }
        finally
        {
            initializationLock.Release();
        }
    }

    private static ProductReviewTableEntity Map(ProductReview review) => new()
    {
        PartitionKey = review.ProductPublicId.ToString(),
        RowKey = review.PublicId.ToString(),
        Comment = review.Comment,
        Rating = review.Rating,
        CreatedAt = review.CreatedAt
    };

    private static ProductReview Map(ProductReviewTableEntity entity) =>
        new(
            Guid.Parse(entity.RowKey),
            Guid.Parse(entity.PartitionKey),
            entity.Comment,
            entity.Rating,
            entity.CreatedAt);
}
