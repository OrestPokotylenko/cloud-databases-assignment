using Microsoft.EntityFrameworkCore;

using OnlineStore.Application.Products.Images;
using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Persistence.Products;

public class ProductImageRepository(
    OnlineStoreDbContext context)
    : IProductImageCommandRepository, IProductImageQueryRepository
{
    public Task<bool> ProductExistsAsync(
        Guid productPublicId,
        CancellationToken ct) =>
        context.Products
            .AsNoTracking()
            .AnyAsync(product => product.PublicId == productPublicId, ct);

    public async Task<ProductImageData?> GetAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        var image = await context.ProductImages
            .AsNoTracking()
            .Where(image =>
                image.PublicId == imagePublicId &&
                context.Products.Any(product =>
                    product.Id == image.ProductId &&
                    product.PublicId == productPublicId))
            .SingleOrDefaultAsync(ct);

        return image is null ? null : Map(image);
    }

    public async Task<ProductImageCollectionData> GetAllAsync(
        Guid productPublicId,
        CancellationToken ct)
    {
        var productId = await context.Products
            .AsNoTracking()
            .Where(product => product.PublicId == productPublicId)
            .Select(product => (int?)product.Id)
            .SingleOrDefaultAsync(ct);

        if (productId is null)
        {
            return new ProductImageCollectionData(false, []);
        }

        var images = await context.ProductImages
            .AsNoTracking()
            .Where(image => image.ProductId == productId)
            .OrderBy(image => image.SortOrder)
            .ThenBy(image => image.Id)
            .ToListAsync(ct);

        return new ProductImageCollectionData(
            true,
            images.Select(Map).ToList());
    }

    public async Task<ProductImageData?> AddAsync(
        Guid productPublicId,
        Guid imagePublicId,
        string blobName,
        string contentType,
        long sizeBytes,
        CancellationToken ct)
    {
        var product = await context.Products
            .Include(product => product.Images)
            .SingleOrDefaultAsync(
                product => product.PublicId == productPublicId,
                ct);
        if (product is null)
        {
            return null;
        }

        var existingImage = product.Images
            .SingleOrDefault(image => image.PublicId == imagePublicId);
        if (existingImage is not null)
        {
            return Map(existingImage);
        }

        var image = product.AddImage(
            imagePublicId,
            blobName,
            contentType,
            sizeBytes);
        await context.SaveChangesAsync(ct);

        return Map(image);
    }

    public async Task<bool> DeleteAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        var product = await context.Products
            .Include(product => product.Images)
            .SingleOrDefaultAsync(
                product => product.PublicId == productPublicId,
                ct);
        if (product is null ||
            product.Images.All(image => image.PublicId != imagePublicId))
        {
            return false;
        }

        var removesPrimaryImage = product.Images
            .Single(image => image.PublicId == imagePublicId)
            .IsPrimary;
        product.RemoveImage(imagePublicId);

        if (removesPrimaryImage)
        {
            await context.ProductImages
                .Where(image =>
                    image.ProductId == product.Id &&
                    image.IsPrimary)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        image => image.IsPrimary,
                        false),
                    ct);
        }

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    public async Task<bool> SetPrimaryAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        var product = await context.Products
            .Include(product => product.Images)
            .SingleOrDefaultAsync(
                product => product.PublicId == productPublicId,
                ct);
        if (product is null ||
            product.Images.All(image => image.PublicId != imagePublicId))
        {
            return false;
        }

        if (product.Images.Single(image => image.PublicId == imagePublicId).IsPrimary)
        {
            return true;
        }

        product.SetPrimaryImage(imagePublicId);

        await context.ProductImages
            .Where(image =>
                image.ProductId == product.Id &&
                image.IsPrimary)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    image => image.IsPrimary,
                    false),
                ct);

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    private static ProductImageData Map(ProductImage image) => new(
        image.PublicId,
        image.BlobName,
        image.ContentType,
        image.SizeBytes,
        image.IsPrimary,
        image.SortOrder);
}
