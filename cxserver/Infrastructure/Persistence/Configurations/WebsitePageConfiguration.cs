using cxserver.Domain.WebEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class WebsitePageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("website_pages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Slug)
            .HasColumnName("slug")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.SeoTitle)
            .HasColumnName("seo_title")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.SeoDescription)
            .HasColumnName("seo_description")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.IsPublished)
            .HasColumnName("is_published")
            .IsRequired();

        builder.Property(x => x.PublishedAtUtc)
            .HasColumnName("published_at_utc");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.Property(x => x.DeletedAtUtc)
            .HasColumnName("deleted_at_utc");

        builder.HasMany(x => x.Sections)
            .WithOne()
            .HasForeignKey(x => x.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Sections)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.Slug)
            .HasDatabaseName("ux_website_pages_slug")
            .IsUnique();

        builder.HasIndex(x => x.IsPublished)
            .HasDatabaseName("ix_website_pages_is_published");
    }
}
