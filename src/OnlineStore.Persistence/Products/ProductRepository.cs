using Microsoft.EntityFrameworkCore;

using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Application.Products.Images;
using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Persistence.Products;

public class ProductRepository(
    OnlineStoreDbContext context)
    : IProductCommandRepository, IProductQueryRepository
{
    public Task<bool> ExistsAsync(
        Guid publicId,
        CancellationToken ct) =>
        context.Products
            .AsNoTracking()
            .AnyAsync(product => product.PublicId == publicId, ct);

    public async Task<Guid> AddAsync(
        Product product,
        CancellationToken ct)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync(ct);

        return product.PublicId;
    }

    public Task<ProductData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct)
    {
        return context.Products
            .AsNoTracking()
            .Where(product => product.PublicId == publicId)
            .Select(product => new ProductData(
                product.PublicId,
                product.CreatedAt,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.Images
                    .OrderBy(image => image.SortOrder)
                    .ThenBy(image => image.Id)
                    .Select(image => new ProductImageData(
                        image.PublicId,
                        image.BlobName,
                        image.ContentType,
                        image.SizeBytes,
                        image.IsPrimary,
                        image.SortOrder))
                    .ToList()))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<ProductPageData> GetPageAsync(
        int skip,
        int take,
        CancellationToken ct)
    {
        var query = context.Products.AsNoTracking();
        var totalCount = await query.CountAsync(ct);
        var products = await query
            .OrderByDescending(product => product.CreatedAt)
            .ThenByDescending(product => product.Id)
            .Skip(skip)
            .Take(take)
            .Select(product => new ProductData(
                product.PublicId,
                product.CreatedAt,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.Images
                    .OrderBy(image => image.SortOrder)
                    .ThenBy(image => image.Id)
                    .Select(image => new ProductImageData(
                        image.PublicId,
                        image.BlobName,
                        image.ContentType,
                        image.SizeBytes,
                        image.IsPrimary,
                        image.SortOrder))
                    .ToList()))
            .ToListAsync(ct);

        return new ProductPageData(products, totalCount);
    }

    public async Task<bool> UpdateAsync(
        ProductUpdate productUpdate,
        CancellationToken ct)
    {
        var product = await context.Products
            .SingleOrDefaultAsync(
                product => product.PublicId == productUpdate.PublicId,
                ct);

        if (product is null)
        {
            return false;
        }

        product.Update(productUpdate.Details);

        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid publicId,
        CancellationToken ct)
    {
        var deletedRows = await context.Products
            .Where(product => product.PublicId == publicId)
            .ExecuteDeleteAsync(ct);

        return deletedRows > 0;
    }
}
