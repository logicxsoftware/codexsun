using cxserver.Domain.BlogEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("blog_posts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(512).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(512).IsRequired();
        builder.Property(x => x.Excerpt).HasColumnName("excerpt").HasMaxLength(2000);
        builder.Property(x => x.Body).HasColumnName("body").HasColumnType("text").IsRequired();
        builder.Property(x => x.FeaturedImage).HasColumnName("featured_image").HasMaxLength(1024);
        builder.Property(x => x.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        var metaKeywordsConverter = new ValueConverter<JsonDocument?, string?>(
            value => value == null ? null : value.RootElement.GetRawText(),
            value => string.IsNullOrWhiteSpace(value) ? null : JsonDocument.Parse(value));

        builder.Property(x => x.MetaKeywords)
            .HasColumnName("meta_keywords")
            .HasColumnType("longtext")
            .HasConversion(metaKeywordsConverter);
        builder.Property(x => x.Published).HasColumnName("published").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.Active).HasColumnName("active").HasDefaultValue(true).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at_utc");

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PostTags)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique().HasDatabaseName("ux_blog_posts_tenant_slug");
        builder.HasIndex(x => new { x.TenantId, x.Published, x.Active, x.CreatedAtUtc }).HasDatabaseName("ix_blog_posts_visibility");
        builder.HasIndex(x => x.CategoryId).HasDatabaseName("ix_blog_posts_category_id");
    }
}
