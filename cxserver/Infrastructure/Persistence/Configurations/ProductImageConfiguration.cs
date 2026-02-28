using cxserver.Domain.ProductCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(x => x.Order)
            .HasColumnName("display_order")
            .IsRequired();

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_product_images_product_id");

        builder.HasIndex(x => new { x.ProductId, x.Order })
            .IsUnique()
            .HasDatabaseName("ux_product_images_product_order");
    }
}
