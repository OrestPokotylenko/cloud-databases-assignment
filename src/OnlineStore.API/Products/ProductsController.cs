using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using OnlineStore.API.ExceptionHandling;
using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.CreateProduct;
using OnlineStore.Application.Products.DeleteProduct;
using OnlineStore.Application.Products.GetProductById;
using OnlineStore.Application.Products.GetProducts;
using OnlineStore.Application.Products.UpdateProduct;

namespace OnlineStore.API.Products;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/products")]
[ApiController]
public class ProductsController(
    IRequestSender sender)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<GetProductsResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetProductsResult>> GetAll(
        CancellationToken ct,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await sender.SendAsync(
            new GetProductsQuery(pageNumber, pageSize),
            ct);

        return Ok(result);
    }

    [HttpGet("{publicId:guid}")]
    [ProducesResponseType<GetProductByIdResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProductByIdResult>> GetById(
        Guid publicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new GetProductByIdQuery(publicId),
            ct);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<CreateProductResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateProductResult>> Create(
        ProductRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new CreateProductCommand(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity),
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new { version = "1", publicId = result.PublicId },
            result);
    }

    [HttpPut("{publicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid publicId,
        ProductRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new UpdateProductCommand(
                publicId,
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity),
            ct);

        return result.Updated ? NoContent() : NotFound();
    }

    [HttpDelete("{publicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid publicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new DeleteProductCommand(publicId),
            ct);

        return result.Deleted ? NoContent() : NotFound();
    }
}
