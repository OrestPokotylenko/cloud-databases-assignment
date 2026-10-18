namespace OnlineStore.Application.Products.Images;

public record ProductImageCollectionData(
    bool ProductExists,
    IReadOnlyList<ProductImageData> Images);
