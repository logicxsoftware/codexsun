using cxserver.Domain.MenuEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id");

        builder.Property(x => x.MenuGroupId)
            .HasColumnName("menu_group_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasColumnName("slug")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Variant)
            .HasColumnName("variant")
            .IsRequired();

        builder.Property(x => x.IsMegaMenu)
            .HasColumnName("is_mega_menu")
            .IsRequired();

        builder.Property(x => x.Order)
            .HasColumnName("display_order")
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

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.MenuGroupId, x.Order, x.IsDeleted })
            .HasDatabaseName("ux_menus_group_order_soft_delete")
            .IsUnique();

        builder.HasIndex(x => new { x.TenantId, x.MenuGroupId, x.Slug, x.IsDeleted })
            .HasDatabaseName("ux_menus_tenant_group_slug_soft_delete")
            .IsUnique();

        builder.HasIndex(x => new { x.MenuGroupId, x.IsActive, x.IsDeleted })
            .HasDatabaseName("ix_menus_group_active");
    }
}
