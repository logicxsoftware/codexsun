using cxserver.Domain.Common;

namespace cxserver.Domain.MenuEngine;

public sealed class MenuItem : ISoftDeletable
{
    private MenuItem(
        Guid id,
        Guid? tenantId,
        Guid menuId,
        Guid? parentId,
        string title,
        string slug,
        string url,
        MenuItemTarget target,
        string? icon,
        string? description,
        int order,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        Id = id;
        TenantId = tenantId;
        MenuId = menuId;
        ParentId = parentId;
        Title = title;
        Slug = slug;
        Url = url;
        Target = target;
        Icon = icon;
        Description = description;
        Order = order;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private MenuItem()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Url = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid? TenantId { get; private set; }
    public Guid MenuId { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Title { get; private set; }
    public string Slug { get; private set; }
    public string Url { get; private set; }
    public MenuItemTarget Target { get; private set; }
    public string? Icon { get; private set; }
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    internal static MenuItem Create(
        Guid id,
        Guid? tenantId,
        Guid menuId,
        Guid? parentId,
        string title,
        string slug,
        string url,
        MenuItemTarget target,
        string? icon,
        string? description,
        int order,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        ValidateTitle(title);
        ValidateSlug(slug);
        ValidateUrl(url);
        ValidateOrder(order);

        return new MenuItem(
            id,
            tenantId,
            menuId,
            parentId,
            title.Trim(),
            NormalizeSlug(slug),
            NormalizeUrl(url),
            target,
            icon?.Trim(),
            description?.Trim(),
            order,
            isActive,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    internal void Update(
        Guid? parentId,
        string title,
        string slug,
        string url,
        MenuItemTarget target,
        string? icon,
        string? description,
        int order,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        ValidateTitle(title);
        ValidateSlug(slug);
        ValidateUrl(url);
        ValidateOrder(order);

        ParentId = parentId;
        Title = title.Trim();
        Slug = NormalizeSlug(slug);
        Url = NormalizeUrl(url);
        Target = target;
        Icon = icon?.Trim();
        Description = description?.Trim();
        Order = order;
        IsActive = isActive;
        UpdatedAtUtc = nowUtc;
    }

    internal void Reorder(Guid? parentId, int order, DateTimeOffset nowUtc)
    {
        ValidateOrder(order);
        ParentId = parentId;
        Order = order;
        UpdatedAtUtc = nowUtc;
    }

    public void Delete(DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
        UpdatedAtUtc = deletedAtUtc;
    }

    public void Restore(DateTimeOffset restoredAtUtc)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = restoredAtUtc;
    }

    private static void ValidateTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Title is required.", nameof(value));
        }
    }

    private static void ValidateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Slug is required.", nameof(value));
        }
    }

    private static void ValidateUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Url is required.", nameof(value));
        }
    }

    private static void ValidateOrder(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Order must be greater than or equal to zero.");
        }
    }

    private static string NormalizeSlug(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static string NormalizeUrl(string value)
    {
        return value.Trim();
    }
}
