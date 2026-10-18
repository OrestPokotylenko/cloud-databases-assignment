using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.GetProductImages;

public record GetProductImagesResult(
    bool ProductExists,
    IReadOnlyList<ProductImageResult> Images);
