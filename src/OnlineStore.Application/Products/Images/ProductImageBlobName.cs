namespace OnlineStore.Application.Products.Images;

public static class ProductImageBlobName
{
    public static string Create(
        Guid productPublicId,
        Guid imagePublicId) =>
        $"{productPublicId:D}/{imagePublicId:D}";
}
