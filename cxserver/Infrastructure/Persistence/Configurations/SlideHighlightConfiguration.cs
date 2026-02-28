using cxserver.Domain.SliderEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class SlideHighlightConfiguration : IEntityTypeConfiguration<SlideHighlight>
{
    public void Configure(EntityTypeBuilder<SlideHighlight> builder)
    {
        builder.ToTable("highlights");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.SlideId).HasColumnName("slide_id").IsRequired();
        builder.Property(x => x.Text).HasColumnName("text").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Variant).HasColumnName("variant").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Order).HasColumnName("display_order").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at_utc");

        builder.HasIndex(x => new { x.SlideId, x.Order, x.IsDeleted })
            .HasDatabaseName("ux_highlights_order_soft_delete")
            .IsUnique();
    }
}
