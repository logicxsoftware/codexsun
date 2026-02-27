using cxserver.Domain.WebEngine;
using cxserver.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class WebsitePageSectionConfiguration : IEntityTypeConfiguration<PageSection>
{
    public void Configure(EntityTypeBuilder<PageSection> builder)
    {
        builder.ToTable("website_page_sections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PageId)
            .HasColumnName("page_id")
            .IsRequired();

        builder.Property(x => x.SectionType)
            .HasColumnName("section_type")
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .HasColumnName("display_order")
            .IsRequired();

        builder.Property(x => x.SectionData)
            .HasColumnName("section_data_json")
            .HasConversion(new JsonDocumentValueConverter())
            .HasColumnType("json")
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

        builder.HasIndex(x => x.PageId)
            .HasDatabaseName("ix_website_page_sections_page_id");

        builder.HasIndex(x => x.DisplayOrder)
            .HasDatabaseName("ix_website_page_sections_display_order");

        builder.HasIndex(x => new { x.PageId, x.DisplayOrder, x.IsDeleted })
            .HasDatabaseName("ux_website_page_sections_page_order_soft_delete")
            .IsUnique();

        builder.HasIndex(x => new { x.PageId, x.IsPublished, x.IsDeleted })
            .HasDatabaseName("ix_website_page_sections_published_lookup");
    }
}
