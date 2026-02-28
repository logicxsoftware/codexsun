using cxserver.Domain.AboutPage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("about_team_members");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.SectionId)
            .HasColumnName("section_id")
            .IsRequired();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasColumnName("bio")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.Image)
            .HasColumnName("image")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.Order)
            .HasColumnName("display_order")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("ix_about_team_members_tenant_id");

        builder.HasIndex(x => x.Order)
            .HasDatabaseName("ix_about_team_members_display_order");

        builder.HasIndex(x => new { x.SectionId, x.Order })
            .HasDatabaseName("ux_about_team_members_section_order")
            .IsUnique();
    }
}
