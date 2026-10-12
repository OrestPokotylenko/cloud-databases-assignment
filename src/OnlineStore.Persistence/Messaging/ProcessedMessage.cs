namespace OnlineStore.Persistence.Messaging;

public class ProcessedMessage
{
    private string? messageType;

    public int Id { get; private set; }
    public Guid MessageId { get; private set; }
    public string MessageType
    {
        get => messageType ?? throw new InvalidOperationException("The message type has not been initialized.");
        private set => messageType = value;
    }
    public DateTimeOffset ProcessedAt { get; private set; }

    private ProcessedMessage() { }

    public ProcessedMessage(
        Guid messageId,
        string messageType,
        DateTimeOffset processedAt)
    {
        MessageId = messageId;
        MessageType = messageType;
        ProcessedAt = processedAt;
    }
}
