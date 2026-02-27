using cxserver.Domain.Tenancy;
using cxserver.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Domain)
            .HasColumnName("domain")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.DatabaseName)
            .HasColumnName("database_name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.ConnectionString)
            .HasColumnName("connection_string")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(x => x.FeatureSettings)
            .HasColumnName("feature_settings_json")
            .HasConversion(new JsonDocumentValueConverter())
            .HasColumnType("json")
            .IsRequired();

        builder.Property(x => x.IsolationMetadata)
            .HasColumnName("isolation_metadata_json")
            .HasConversion(new JsonDocumentValueConverter())
            .HasColumnType("json")
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

        builder.HasIndex(x => x.Identifier)
            .HasDatabaseName("ux_tenants_identifier")
            .IsUnique();

        builder.HasIndex(x => x.Domain)
            .HasDatabaseName("ux_tenants_domain")
            .IsUnique();

        builder.HasIndex(x => x.DatabaseName)
            .HasDatabaseName("ux_tenants_database_name")
            .IsUnique();

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_tenants_status");

        builder.HasIndex(x => x.IsDeleted)
            .HasDatabaseName("ix_tenants_is_deleted");

        builder.HasIndex(x => new { x.Identifier, x.Status, x.IsDeleted })
            .HasDatabaseName("ix_tenants_identifier_status_soft_delete");
    }
}
