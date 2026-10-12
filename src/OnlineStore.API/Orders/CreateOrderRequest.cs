using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.Orders;

public class CreateOrderRequest
{
    public Guid CustomerPublicId { get; init; }

    [Required]
    public required OrderCustomerRequest Customer { get; init; }

    [Required]
    public required DeliveryAddressRequest DeliveryAddress { get; init; }

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<OrderItemRequest> Items { get; init; }
}
