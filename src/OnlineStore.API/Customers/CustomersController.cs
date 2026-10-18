using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using OnlineStore.API.ExceptionHandling;
using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Customers;
using OnlineStore.Application.Customers.CreateCustomer;
using OnlineStore.Application.Customers.GetCustomer;
using OnlineStore.Application.Customers.UpdateCustomer;

namespace OnlineStore.API.Customers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/customers")]
[ApiController]
public class CustomersController(
    IRequestSender sender)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateCustomerResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateCustomerResult>> Create(
        CreateCustomerRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new CreateCustomerCommand(
                request.FirstName,
                request.LastName,
                request.Email),
            ct);

        return result.PublicId is { } publicId
            ? CreatedAtAction(
                nameof(GetById),
                new { version = "1", publicId },
                result)
            : Conflict();
    }

    [HttpGet("{publicId:guid}")]
    [ProducesResponseType<CustomerResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResult>> GetById(
        Guid publicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new GetCustomerQuery(publicId),
            ct);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{publicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid publicId,
        UpdateCustomerRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new UpdateCustomerCommand(
                publicId,
                request.FirstName,
                request.LastName,
                request.Email),
            ct);

        if (!result.CustomerExists)
        {
            return NotFound();
        }

        return result.EmailAvailable ? NoContent() : Conflict();
    }
}
