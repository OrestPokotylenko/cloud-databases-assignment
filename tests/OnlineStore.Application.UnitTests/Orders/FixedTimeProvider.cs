namespace OnlineStore.Application.UnitTests.Orders;

internal class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
