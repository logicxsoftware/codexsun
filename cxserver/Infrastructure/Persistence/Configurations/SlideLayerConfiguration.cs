using cxserver.Domain.SliderEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class SlideLayerConfiguration : IEntityTypeConfiguration<SlideLayer>
{
    public void Configure(EntityTypeBuilder<SlideLayer> builder)
    {
        builder.ToTable("slide_layers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.SlideId).HasColumnName("slide_id").IsRequired();
        builder.Property(x => x.Order).HasColumnName("display_order").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").IsRequired();
        builder.Property(x => x.Content).HasColumnName("content").HasMaxLength(2048).IsRequired();
        builder.Property(x => x.MediaUrl).HasColumnName("media_url").HasMaxLength(2048);
        builder.Property(x => x.PositionX).HasColumnName("position_x").HasPrecision(6, 2).IsRequired();
        builder.Property(x => x.PositionY).HasColumnName("position_y").HasPrecision(6, 2).IsRequired();
        builder.Property(x => x.Width).HasColumnName("width").HasMaxLength(32).IsRequired();
        builder.Property(x => x.AnimationFrom).HasColumnName("animation_from").IsRequired();
        builder.Property(x => x.AnimationDelay).HasColumnName("animation_delay").IsRequired();
        builder.Property(x => x.AnimationDuration).HasColumnName("animation_duration").IsRequired();
        builder.Property(x => x.AnimationEasing).HasColumnName("animation_easing").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ResponsiveVisibility).HasColumnName("responsive_visibility").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at_utc");

        builder.HasIndex(x => new { x.SlideId, x.Order, x.IsDeleted })
            .HasDatabaseName("ux_slide_layers_order_soft_delete")
            .IsUnique();
    }
}
