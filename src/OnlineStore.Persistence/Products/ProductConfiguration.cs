using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Persistence.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.PublicId)
            .IsRequired()
            .HasDefaultValueSql("NEWID()")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.StockQuantity)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .ValueGeneratedOnAdd();

        builder.HasIndex(e => e.PublicId)
            .IsUnique();

        builder.HasIndex(e => new { e.CreatedAt, e.Id })
            .IsDescending();

        builder.HasMany(e => e.Images)
            .WithOne()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Products", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Products_Price_NonNegative",
                "[Price] >= 0");
            tableBuilder.HasCheckConstraint(
                "CK_Products_StockQuantity_NonNegative",
                "[StockQuantity] >= 0");
        });
    }
}
