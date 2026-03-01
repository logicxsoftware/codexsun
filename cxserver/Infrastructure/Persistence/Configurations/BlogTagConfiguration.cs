using cxserver.Domain.BlogEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class BlogTagConfiguration : IEntityTypeConfiguration<BlogTag>
{
    public void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        builder.ToTable("blog_tags");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Active).HasColumnName("active").IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at_utc");

        builder.HasIndex(x => x.TenantId).HasDatabaseName("ix_blog_tags_tenant_id");
        builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique().HasDatabaseName("ux_blog_tags_tenant_name");
        builder.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique().HasDatabaseName("ux_blog_tags_tenant_slug");
    }
}

