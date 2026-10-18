using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using OnlineStore.API.ExceptionHandling;
using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.ProductReviews;
using OnlineStore.Application.ProductReviews.CreateProductReview;
using OnlineStore.Application.ProductReviews.DeleteProductReview;
using OnlineStore.Application.ProductReviews.GetProductReviews;

namespace OnlineStore.API.ProductReviews;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/products/{productPublicId:guid}/reviews")]
[ApiController]
public class ProductReviewsController(
    IRequestSender sender)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ProductReviewResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductReviewResult>> Create(
        Guid productPublicId,
        CreateProductReviewRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new CreateProductReviewCommand(
                productPublicId,
                request.Comment,
                request.Rating),
            ct);

        return result is { ProductExists: true, Review: { } review }
            ? StatusCode(StatusCodes.Status201Created, review)
            : NotFound();
    }

    [HttpGet]
    [ProducesResponseType<ProductReviewPageResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductReviewPageResult>> GetAll(
        Guid productPublicId,
        CancellationToken ct,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? continuationToken = null)
    {
        var result = await sender.SendAsync(
            new GetProductReviewsQuery(
                productPublicId,
                pageSize,
                continuationToken),
            ct);

        return result is { ProductExists: true, Page: { } page }
            ? Ok(page)
            : NotFound();
    }

    [HttpDelete("{reviewPublicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid productPublicId,
        Guid reviewPublicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new DeleteProductReviewCommand(
                productPublicId,
                reviewPublicId),
            ct);

        return result.Deleted ? NoContent() : NotFound();
    }
}
