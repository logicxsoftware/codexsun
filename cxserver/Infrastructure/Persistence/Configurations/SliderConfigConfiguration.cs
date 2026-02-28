using cxserver.Domain.SliderEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class SliderConfigConfiguration : IEntityTypeConfiguration<SliderConfig>
{
    public void Configure(EntityTypeBuilder<SliderConfig> builder)
    {
        builder.ToTable("slider_configs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.HeightMode).HasColumnName("height_mode").IsRequired();
        builder.Property(x => x.HeightValue).HasColumnName("height_value").IsRequired();
        builder.Property(x => x.ContainerMode).HasColumnName("container_mode").IsRequired();
        builder.Property(x => x.ContentAlignment).HasColumnName("content_alignment").IsRequired();
        builder.Property(x => x.Autoplay).HasColumnName("autoplay").IsRequired();
        builder.Property(x => x.Loop).HasColumnName("loop").IsRequired();
        builder.Property(x => x.ShowProgress).HasColumnName("show_progress").IsRequired();
        builder.Property(x => x.ShowNavArrows).HasColumnName("show_nav_arrows").IsRequired();
        builder.Property(x => x.ShowDots).HasColumnName("show_dots").IsRequired();
        builder.Property(x => x.Parallax).HasColumnName("parallax").IsRequired();
        builder.Property(x => x.Particles).HasColumnName("particles").IsRequired();
        builder.Property(x => x.DefaultVariant).HasColumnName("default_variant").IsRequired();
        builder.Property(x => x.DefaultIntensity).HasColumnName("default_intensity").IsRequired();
        builder.Property(x => x.DefaultDirection).HasColumnName("default_direction").IsRequired();
        builder.Property(x => x.DefaultBackgroundMode).HasColumnName("default_background_mode").IsRequired();
        builder.Property(x => x.ScrollBehavior).HasColumnName("scroll_behavior").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.DeletedAtUtc).HasColumnName("deleted_at_utc");

        builder.HasMany(x => x.Slides)
            .WithOne()
            .HasForeignKey(x => x.SliderConfigId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.IsDeleted })
            .HasDatabaseName("ux_slider_configs_tenant_soft_delete")
            .IsUnique();
    }
}
