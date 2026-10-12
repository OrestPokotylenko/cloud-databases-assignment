using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Orders.Abstractions;

namespace OnlineStore.Application.Orders.GetOrderById;

public class GetOrderByIdHandler(
    IOrderQueryRepository orderRepository)
    : IHandler<GetOrderByIdQuery, GetOrderByIdResult?>
{
    public async Task<GetOrderByIdResult?> HandleAsync(
        GetOrderByIdQuery request,
        CancellationToken ct)
    {
        var order = await orderRepository.GetByPublicIdAsync(request.PublicId, ct);
        return order is null ? null : Map(order);
    }

    private static GetOrderByIdResult Map(OrderData order) => new(
        order.PublicId,
        order.CustomerPublicId,
        order.CustomerFullName,
        order.CustomerEmail,
        order.CustomerPhoneNumber,
        order.Street,
        order.HouseNumber,
        order.AddressAddition,
        order.PostalCode,
        order.City,
        order.CountryCode,
        order.Status,
        order.OrderedAt,
        order.UpdatedAt,
        order.ShippedAt,
        order.ShippedAt is null
            ? null
            : (order.ShippedAt.Value - order.OrderedAt).TotalSeconds,
        order.TotalAmount,
        order.RejectionReason,
        order.Items.Select(item => new OrderItemResult(
            item.ProductPublicId,
            item.ProductName,
            item.UnitPrice,
            item.Quantity,
            item.LineTotal)).ToList());
}
