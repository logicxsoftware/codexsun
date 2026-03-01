using cxserver.Domain.BlogEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class BlogPostImageConfiguration : IEntityTypeConfiguration<BlogPostImage>
{
    public void Configure(EntityTypeBuilder<BlogPostImage> builder)
    {
        builder.ToTable("blog_post_images");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.PostId).HasColumnName("post_id").IsRequired();
        builder.Property(x => x.ImagePath).HasColumnName("image_path").HasMaxLength(1024).IsRequired();
        builder.Property(x => x.AltText).HasColumnName("alt_text").HasMaxLength(512);
        builder.Property(x => x.Caption).HasColumnName("caption").HasMaxLength(1024);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasIndex(x => new { x.PostId, x.SortOrder }).HasDatabaseName("ix_blog_post_images_post_sort");
        builder.HasIndex(x => x.TenantId).HasDatabaseName("ix_blog_post_images_tenant_id");
    }
}

