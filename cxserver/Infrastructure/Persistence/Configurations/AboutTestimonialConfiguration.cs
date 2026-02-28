using cxserver.Domain.AboutPage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class AboutTestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.ToTable("about_testimonials");

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

        builder.Property(x => x.Company)
            .HasColumnName("company")
            .HasMaxLength(256);

        builder.Property(x => x.Quote)
            .HasColumnName("quote")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasColumnName("rating");

        builder.Property(x => x.Order)
            .HasColumnName("display_order")
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("ix_about_testimonials_tenant_id");

        builder.HasIndex(x => x.Order)
            .HasDatabaseName("ix_about_testimonials_display_order");

        builder.HasIndex(x => new { x.SectionId, x.Order })
            .HasDatabaseName("ux_about_testimonials_section_order")
            .IsUnique();
    }
}
