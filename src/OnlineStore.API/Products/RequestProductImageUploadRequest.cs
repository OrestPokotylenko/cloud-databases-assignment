using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.Products;

public class RequestProductImageUploadRequest
{
    [Required]
    public required string ContentType { get; init; }

    [Range(1, long.MaxValue)]
    public long SizeBytes { get; init; }
}
