using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Persistence.Products;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.PublicId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.ProductId)
            .IsRequired();

        builder.Property(e => e.BlobName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.SizeBytes)
            .IsRequired();

        builder.Property(e => e.IsPrimary)
            .IsRequired();

        builder.Property(e => e.SortOrder)
            .IsRequired();

        builder.HasIndex(e => e.PublicId)
            .IsUnique();

        builder.HasIndex(e => e.BlobName)
            .IsUnique();

        builder.HasIndex(e => e.ProductId)
            .IsUnique()
            .HasFilter("[IsPrimary] = 1");

        builder.ToTable("ProductImages", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_ProductImages_SizeBytes_Positive",
                "[SizeBytes] > 0");
            tableBuilder.HasCheckConstraint(
                "CK_ProductImages_SortOrder_NonNegative",
                "[SortOrder] >= 0");
        });
    }
}
