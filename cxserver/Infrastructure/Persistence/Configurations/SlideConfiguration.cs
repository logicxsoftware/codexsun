using cxserver.Domain.SliderEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class SlideConfiguration : IEntityTypeConfiguration<Slide>
{
    public void Configure(EntityTypeBuilder<Slide> builder)
    {
        builder.ToTable("slides");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.SliderConfigId).HasColumnName("slider_config_id").IsRequired();
        builder.Property(x => x.Order).HasColumnName("display_order").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Tagline).HasColumnName("tagline").HasMaxLength(1024).IsRequired();
        builder.Property(x => x.ActionText).HasColumnName("action_text").HasMaxLength(128);
        builder.Property(x => x.ActionHref).HasColumnName("action_href").HasMaxLength(2048);
        builder.Property(x => x.CtaColor).HasColumnName("cta_color").IsRequired();
        builder.Property(x => x.Duration).HasColumnName("duration").IsRequired();
        builder.Property(x => x.Direction).HasColumnName("direction").IsRequired();
        builder.Property(x => x.Variant).HasColumnName("variant").IsRequired();
        builder.Property(x => x.Intensity).HasColumnName("intensity").IsRequired();
        builder.Property(x => x.BackgroundMode).HasColumnName("background_mode").IsRequired();
        builder.Property(x => x.ShowOverlay).HasColumnName("show_overlay").IsRequired();
        builder.Property(x => x.OverlayToken).HasColumnName("overlay_token").HasMaxLength(128).IsRequired();
        builder.Property(x => x.BackgroundUrl).HasColumnName("background_url").HasMaxLength(2048).IsRequired();
        builder.Property(x => x.MediaType).HasColumnName("media_type").IsRequired();
        builder.Property(x => x.YoutubeVideoId).HasColumnName("youtube_video_id").HasMaxLength(64);
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at_utc");

        builder.HasMany(x => x.Layers)
            .WithOne()
            .HasForeignKey(x => x.SlideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Highlights)
            .WithOne()
            .HasForeignKey(x => x.SlideId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.SliderConfigId, x.IsActive, x.IsDeleted })
            .HasDatabaseName("ix_slides_lookup");

        builder.HasIndex(x => new { x.SliderConfigId, x.Order, x.IsDeleted })
            .HasDatabaseName("ux_slides_order_soft_delete")
            .IsUnique();
    }
}
