using cxserver.Domain.AboutPage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class RoadmapMilestoneConfiguration : IEntityTypeConfiguration<RoadmapMilestone>
{
    public void Configure(EntityTypeBuilder<RoadmapMilestone> builder)
    {
        builder.ToTable("about_roadmap_milestones");

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

        builder.Property(x => x.Year)
            .HasColumnName("year")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.Order)
            .HasColumnName("display_order")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("ix_about_roadmap_milestones_tenant_id");

        builder.HasIndex(x => x.Order)
            .HasDatabaseName("ix_about_roadmap_milestones_display_order");

        builder.HasIndex(x => new { x.SectionId, x.Order })
            .HasDatabaseName("ux_about_roadmap_milestones_section_order")
            .IsUnique();
    }
}
