namespace OnlineStore.Application.ProductReviews;

public record ProductReviewResult(
    Guid PublicId,
    string Comment,
    int? Rating,
    DateTimeOffset CreatedAt);
