namespace OnlineStore.ExternalServices.Options;

public sealed class QueueStorageOptions
{
    public const string SectionName = "Queues";

    public required string ConnectionString { get; init; }
    public required string CreateOrderQueueName { get; init; }
    public required string OrderUpdateQueueName { get; init; }
}
