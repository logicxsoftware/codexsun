using cxserver.Domain.MenuEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxserver.Infrastructure.Persistence.Configurations;

internal sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id");

        builder.Property(x => x.MenuId)
            .HasColumnName("menu_id")
            .IsRequired();

        builder.Property(x => x.ParentId)
            .HasColumnName("parent_id");

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasColumnName("slug")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Url)
            .HasColumnName("url")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(x => x.Target)
            .HasColumnName("target")
            .IsRequired();

        builder.Property(x => x.Icon)
            .HasColumnName("icon")
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(512);

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

        builder.HasIndex(x => new { x.MenuId, x.ParentId, x.Order, x.IsDeleted })
            .HasDatabaseName("ux_menu_items_menu_parent_order_soft_delete")
            .IsUnique();

        builder.HasIndex(x => new { x.MenuId, x.Slug, x.IsDeleted })
            .HasDatabaseName("ux_menu_items_menu_slug_soft_delete")
            .IsUnique();

        builder.HasIndex(x => new { x.MenuId, x.ParentId, x.IsActive, x.IsDeleted })
            .HasDatabaseName("ix_menu_items_tree_lookup");

        builder.HasOne<MenuItem>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
