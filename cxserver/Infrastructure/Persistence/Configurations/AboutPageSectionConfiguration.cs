using cxserver.Domain.AboutPage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class AboutPageSectionConfiguration : IEntityTypeConfiguration<AboutPageSection>
{
    public void Configure(EntityTypeBuilder<AboutPageSection> builder)
    {
        builder.ToTable("about_page_sections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(x => x.HeroTitle)
            .HasColumnName("hero_title")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.HeroSubtitle)
            .HasColumnName("hero_subtitle")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.AboutTitle)
            .HasColumnName("about_title")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.AboutSubtitle)
            .HasColumnName("about_subtitle")
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.HasMany(x => x.TeamMembers)
            .WithOne()
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Testimonials)
            .WithOne()
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RoadmapMilestones)
            .WithOne()
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TeamMembers)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Testimonials)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.RoadmapMilestones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("ix_about_page_sections_tenant_id");

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("ux_about_page_sections_tenant_id")
            .IsUnique();
    }
}
