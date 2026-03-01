using cxserver.Domain.BlogEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class BlogLikeConfiguration : IEntityTypeConfiguration<BlogLike>
{
    public void Configure(EntityTypeBuilder<BlogLike> builder)
    {
        builder.ToTable("blog_likes");

        builder.HasKey(x => new { x.PostId, x.UserId });
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.PostId).HasColumnName("post_id").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.Liked).HasColumnName("liked").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at_utc");

        builder.HasIndex(x => x.TenantId).HasDatabaseName("ix_blog_likes_tenant_id");
    }
}

