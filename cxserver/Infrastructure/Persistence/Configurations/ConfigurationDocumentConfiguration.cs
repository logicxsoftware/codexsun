using cxserver.Domain.Configuration;
using cxserver.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class ConfigurationDocumentConfiguration : IEntityTypeConfiguration<ConfigurationDocument>
{
    public void Configure(EntityTypeBuilder<ConfigurationDocument> builder)
    {
        builder.ToTable("configuration_documents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.NamespaceKey)
            .HasColumnName("namespace_key")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.DocumentKey)
            .HasColumnName("document_key")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnName("payload_json")
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

        builder.HasIndex(x => new { x.NamespaceKey, x.DocumentKey })
            .HasDatabaseName("ux_configuration_documents_namespace_document")
            .IsUnique();

        builder.HasIndex(x => x.IsDeleted)
            .HasDatabaseName("ix_configuration_documents_is_deleted");

        builder.HasIndex(x => new { x.NamespaceKey, x.DocumentKey, x.IsDeleted })
            .HasDatabaseName("ix_configuration_documents_lookup_soft_delete");
    }
}
