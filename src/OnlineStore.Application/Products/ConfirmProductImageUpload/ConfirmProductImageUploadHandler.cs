using Microsoft.Extensions.Options;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.ConfirmProductImageUpload;

public class ConfirmProductImageUploadHandler(
    IProductImageCommandRepository commandRepository,
    IProductImageQueryRepository queryRepository,
    IProductImageStorage storage,
    IOptions<ProductImageValidationOptions> options)
    : IHandler<ConfirmProductImageUploadCommand, ConfirmProductImageUploadResult>
{
    public async Task<ConfirmProductImageUploadResult> HandleAsync(
        ConfirmProductImageUploadCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.ProductPublicId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(request.ImagePublicId, Guid.Empty);

        if (!await queryRepository.ProductExistsAsync(request.ProductPublicId, ct))
        {
            return new ConfirmProductImageUploadResult(false, null);
        }

        var blobName = ProductImageBlobName.Create(
            request.ProductPublicId,
            request.ImagePublicId);
        var properties = await storage.GetPropertiesAsync(blobName, ct)
            ?? throw new ArgumentException("The uploaded image blob does not exist.");
        var contentType = properties.ContentType.Trim().ToLowerInvariant();

        if (!options.Value.SupportedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            await storage.DeleteAsync(blobName, ct);
            throw new ArgumentException("The uploaded image content type is not supported.");
        }

        if (properties.SizeBytes < 1 ||
            properties.SizeBytes > options.Value.MaximumSizeBytes)
        {
            await storage.DeleteAsync(blobName, ct);
            throw new ArgumentException(
                $"The uploaded image must be between 1 and {options.Value.MaximumSizeBytes} bytes.");
        }

        var image = await commandRepository.AddAsync(
            request.ProductPublicId,
            request.ImagePublicId,
            blobName,
            contentType,
            properties.SizeBytes,
            ct);

        return image is null
            ? new ConfirmProductImageUploadResult(false, null)
            : new ConfirmProductImageUploadResult(
                true,
                ProductImageResultMapper.Map(image, storage));
    }
}
