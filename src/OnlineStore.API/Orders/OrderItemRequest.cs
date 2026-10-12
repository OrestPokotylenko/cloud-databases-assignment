using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.Orders;

public class OrderItemRequest
{
    public Guid ProductPublicId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}
