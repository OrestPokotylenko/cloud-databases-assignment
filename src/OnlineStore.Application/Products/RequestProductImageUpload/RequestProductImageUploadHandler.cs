using Microsoft.Extensions.Options;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.RequestProductImageUpload;

public class RequestProductImageUploadHandler(
    IProductImageQueryRepository imageRepository,
    IProductImageStorage storage,
    IOptions<ProductImageValidationOptions> options)
    : IHandler<RequestProductImageUploadCommand, RequestProductImageUploadResult>
{
    public async Task<RequestProductImageUploadResult> HandleAsync(
        RequestProductImageUploadCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.ProductPublicId, Guid.Empty);

        var contentType = request.ContentType?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(contentType) ||
            !options.Value.SupportedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The image content type is not supported.", nameof(request.ContentType));
        }

        if (request.SizeBytes < 1 || request.SizeBytes > options.Value.MaximumSizeBytes)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.SizeBytes),
                $"The image size must be between 1 and {options.Value.MaximumSizeBytes} bytes.");
        }

        if (!await imageRepository.ProductExistsAsync(request.ProductPublicId, ct))
        {
            return new RequestProductImageUploadResult(false, null, null, null);
        }

        var imagePublicId = Guid.NewGuid();
        var blobName = ProductImageBlobName.Create(
            request.ProductPublicId,
            imagePublicId);
        var upload = await storage.CreateUploadAsync(blobName, contentType, ct);

        return new RequestProductImageUploadResult(
            true,
            imagePublicId,
            upload.UploadUrl,
            upload.ExpiresAt);
    }
}
