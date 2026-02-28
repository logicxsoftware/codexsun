using cxserver.Domain.ContactEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("contact_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasColumnName("subject")
            .HasMaxLength(512);

        builder.Property(x => x.Message)
            .HasColumnName("message")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("ix_contact_messages_tenant_id");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_contact_messages_created_at_utc");
    }
}
