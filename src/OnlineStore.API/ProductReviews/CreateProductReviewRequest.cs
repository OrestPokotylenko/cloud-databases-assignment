using System.ComponentModel.DataAnnotations;

using OnlineStore.Domain.ProductReviews;

namespace OnlineStore.API.ProductReviews;

public class CreateProductReviewRequest
{
    [Required]
    [StringLength(ProductReview.MaximumCommentLength)]
    public required string Comment { get; init; }

    [Range(1, 5)]
    public int? Rating { get; init; }
}
