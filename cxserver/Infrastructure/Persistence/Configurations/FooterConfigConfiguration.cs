using cxserver.Domain.NavigationEngine;
using cxserver.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class FooterConfigConfiguration : IEntityTypeConfiguration<FooterConfig>
{
    public void Configure(EntityTypeBuilder<FooterConfig> builder)
    {
        builder.ToTable("footer_configs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id");

        builder.Property(x => x.LayoutConfig)
            .HasColumnName("layout_config_json")
            .HasConversion(new JsonDocumentValueConverter())
            .HasColumnType("json")
            .IsRequired();

        builder.Property(x => x.StyleConfig)
            .HasColumnName("style_config_json")
            .HasConversion(new JsonDocumentValueConverter())
            .HasColumnType("json")
            .IsRequired();

        builder.Property(x => x.BehaviorConfig)
            .HasColumnName("behavior_config_json")
            .HasConversion(new JsonDocumentValueConverter())
            .HasColumnType("json")
            .IsRequired();

        builder.Property(x => x.ComponentConfig)
            .HasColumnName("component_config_json")
            .HasConversion(new JsonDocumentValueConverter())
            .HasColumnType("json")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

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

        builder.HasIndex(x => new { x.TenantId, x.IsDeleted })
            .HasDatabaseName("ux_footer_configs_tenant_soft_delete")
            .IsUnique();

        builder.HasIndex(x => new { x.IsActive, x.IsDeleted })
            .HasDatabaseName("ix_footer_configs_active");
    }
}
