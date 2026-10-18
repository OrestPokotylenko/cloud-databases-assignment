namespace OnlineStore.Application.UnitTests.ProductReviews;

internal class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
