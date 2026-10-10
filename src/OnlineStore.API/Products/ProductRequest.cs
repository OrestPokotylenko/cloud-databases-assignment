using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.Products;

public sealed class ProductRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = null!;

    [StringLength(2000)]
    public string? Description { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }
}
