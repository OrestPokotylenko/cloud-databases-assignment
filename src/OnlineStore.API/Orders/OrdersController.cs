using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

using OnlineStore.API.ExceptionHandling;
using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Orders.CreateOrder;
using OnlineStore.Application.Orders.GetOrderById;
using OnlineStore.Application.Orders.GetOrders;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Application.Orders.UpdateOrderStatus;

namespace OnlineStore.API.Orders;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/orders")]
[ApiController]
public class OrdersController(
    IRequestSender sender)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<GetOrdersResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetOrdersResult>> GetAll(
        CancellationToken ct,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await sender.SendAsync(
            new GetOrdersQuery(pageNumber, pageSize),
            ct);

        return Ok(result);
    }

    [HttpGet("{publicId:guid}")]
    [ProducesResponseType<GetOrderByIdResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetOrderByIdResult>> GetById(
        Guid publicId,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new GetOrderByIdQuery(publicId),
            ct);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<CreateOrderResult>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateOrderResult>> Create(
        CreateOrderRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new CreateOrderCommand(
                request.CustomerPublicId,
                new OrderCustomerMessage(
                    request.Customer.FullName,
                    request.Customer.Email,
                    request.Customer.PhoneNumber),
                new DeliveryAddressMessage(
                    request.DeliveryAddress.Street,
                    request.DeliveryAddress.HouseNumber,
                    request.DeliveryAddress.Addition,
                    request.DeliveryAddress.PostalCode,
                    request.DeliveryAddress.City,
                    request.DeliveryAddress.CountryCode),
                request.Items.Select(item => new OrderItemMessage(
                    item.ProductPublicId,
                    item.Quantity)).ToList()),
            ct);

        return AcceptedAtAction(
            nameof(GetById),
            new { version = "1", publicId = result.PublicId },
            result);
    }

    [HttpPut("{publicId:guid}/status")]
    [ProducesResponseType<UpdateOrderStatusResult>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateOrderStatusResult>> UpdateStatus(
        Guid publicId,
        UpdateOrderStatusRequest request,
        CancellationToken ct)
    {
        var result = await sender.SendAsync(
            new UpdateOrderStatusCommand(publicId, request.Status),
            ct);

        return result.Accepted ? Accepted(result) : NotFound();
    }
}
