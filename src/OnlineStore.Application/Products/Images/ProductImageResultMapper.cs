namespace OnlineStore.Application.Products.Images;

public static class ProductImageResultMapper
{
    public static ProductImageResult Map(
        ProductImageData image,
        IProductImageStorage storage) =>
        new(
            image.PublicId,
            storage.GetPublicUrl(image.BlobName),
            image.ContentType,
            image.SizeBytes,
            image.IsPrimary,
            image.SortOrder);
}
