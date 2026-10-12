using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.Orders;

public class UpdateOrderStatusRequest
{
    [Required]
    public required string Status { get; init; }
}
