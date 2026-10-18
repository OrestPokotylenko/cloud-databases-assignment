using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using OnlineStore.API.ExceptionHandling;
using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.ConfirmProductImageUpload;
using OnlineStore.Application.Products.DeleteProductImage;
using OnlineStore.Application.Products.GetProductImages;
using OnlineStore.Application.Products.Images;
using OnlineStore.Application.Products.RequestProductImageUpload;
using OnlineStore.Application.Products.SetPrimaryProductImage;

namespace OnlineStore.API.Products;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/products/{productPublicId:guid}/images")]
[ApiController]
public class ProductImagesController(
    IRequestSender sender)
    : ControllerBase
{
    [HttpPost("upload")]
    [ProducesResponseType<ProductImageUploadResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductImageUploadResponse>> RequestUpload(
        Guid productPublicId,
        RequestProductImageUploadRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new RequestProductImageUploadCommand(
                productPublicId,
                request.ContentType,
                request.SizeBytes),
            ct);
        if (result is not
            {
                ProductExists: true,
                ImagePublicId: { } imagePublicId,
                UploadUrl: { } uploadUrl,
                ExpiresAt: { } expiresAt
            })
        {
            return NotFound();
        }

        return Ok(new ProductImageUploadResponse(
            imagePublicId,
            uploadUrl,
            expiresAt));
    }

    [HttpPost("{imagePublicId:guid}/confirm")]
    [ProducesResponseType<ProductImageResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductImageResult>> ConfirmUpload(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new ConfirmProductImageUploadCommand(
                productPublicId,
                imagePublicId),
            ct);
        if (!result.ProductExists || result.Image is null)
        {
            return NotFound();
        }

        return CreatedAtAction(
            nameof(GetAll),
            new { version = "1", productPublicId },
            result.Image);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductImageResult>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProductImageResult>>> GetAll(
        Guid productPublicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new GetProductImagesQuery(productPublicId),
            ct);

        return result.ProductExists ? Ok(result.Images) : NotFound();
    }

    [HttpDelete("{imagePublicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new DeleteProductImageCommand(
                productPublicId,
                imagePublicId),
            ct);

        return result.Deleted ? NoContent() : NotFound();
    }

    [HttpPost("{imagePublicId:guid}/primary")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimary(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new SetPrimaryProductImageCommand(
                productPublicId,
                imagePublicId),
            ct);

        return result.Updated ? NoContent() : NotFound();
    }
}
