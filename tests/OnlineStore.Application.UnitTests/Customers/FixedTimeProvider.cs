namespace OnlineStore.Application.UnitTests.Customers;

internal class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
