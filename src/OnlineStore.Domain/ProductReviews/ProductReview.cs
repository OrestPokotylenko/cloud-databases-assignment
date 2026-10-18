namespace OnlineStore.Domain.ProductReviews;

public class ProductReview
{
    public const int MaximumCommentLength = 2000;

    public Guid PublicId { get; }
    public Guid ProductPublicId { get; }
    public string Comment { get; }
    public int? Rating { get; }
    public DateTimeOffset CreatedAt { get; }

    public ProductReview(
        Guid publicId,
        Guid productPublicId,
        string comment,
        int? rating,
        DateTimeOffset createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(publicId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(productPublicId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);

        var normalizedComment = comment.Trim();
        if (normalizedComment.Length > MaximumCommentLength)
        {
            throw new ArgumentException(
                $"The comment cannot exceed {MaximumCommentLength} characters.",
                nameof(comment));
        }

        if (rating is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rating),
                "The rating must be between 1 and 5.");
        }

        if (createdAt == default)
        {
            throw new ArgumentOutOfRangeException(nameof(createdAt));
        }

        PublicId = publicId;
        ProductPublicId = productPublicId;
        Comment = normalizedComment;
        Rating = rating;
        CreatedAt = createdAt;
    }
}
