using Azure;
using Azure.Data.Tables;

namespace OnlineStore.ExternalServices.TableStorage;

internal class ProductReviewTableEntity : ITableEntity
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public required string Comment { get; set; }
    public int? Rating { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}
