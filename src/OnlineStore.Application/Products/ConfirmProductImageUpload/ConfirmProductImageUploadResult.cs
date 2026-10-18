using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.ConfirmProductImageUpload;

public record ConfirmProductImageUploadResult(
    bool ProductExists,
    ProductImageResult? Image);
