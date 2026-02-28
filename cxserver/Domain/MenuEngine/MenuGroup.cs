using cxserver.Domain.Common;

namespace cxserver.Domain.MenuEngine;

public sealed class MenuGroup : AggregateRoot, ISoftDeletable
{
    private readonly List<Menu> _menus;

    private MenuGroup(
        Guid id,
        Guid? tenantId,
        MenuGroupType type,
        string name,
        string slug,
        string? description,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        TenantId = tenantId;
        Type = type;
        Name = name;
        Slug = slug;
        Description = description;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
        _menus = new List<Menu>();
    }

    private MenuGroup() : base(Guid.NewGuid())
    {
        Name = string.Empty;
        Slug = string.Empty;
        _menus = new List<Menu>();
    }

    public Guid? TenantId { get; private set; }
    public MenuGroupType Type { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public IReadOnlyCollection<Menu> Menus => _menus.AsReadOnly();

    public static MenuGroup Create(
        Guid id,
        Guid? tenantId,
        MenuGroupType type,
        string name,
        string slug,
        string? description,
        DateTimeOffset nowUtc)
    {
        ValidateName(name);
        ValidateSlug(slug);

        return new MenuGroup(
            id,
            tenantId,
            type,
            name.Trim(),
            NormalizeSlug(slug),
            description?.Trim(),
            true,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Update(string name, string slug, string? description, bool isActive, DateTimeOffset nowUtc)
    {
        ValidateName(name);
        ValidateSlug(slug);

        Name = name.Trim();
        Slug = NormalizeSlug(slug);
        Description = description?.Trim();
        IsActive = isActive;
        UpdatedAtUtc = nowUtc;
    }

    public Menu AddMenu(
        Guid menuId,
        Guid? tenantId,
        string name,
        string slug,
        MenuVariant variant,
        bool isMegaMenu,
        int order,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        EnsureMenuOrderUnique(order, null);
        EnsureMenuSlugUnique(slug, null);

        var menu = Menu.Create(menuId, tenantId, Id, name, slug, variant, isMegaMenu, order, isActive, nowUtc);
        _menus.Add(menu);
        UpdatedAtUtc = nowUtc;
        NormalizeMenuOrder(nowUtc);
        return menu;
    }

    public Menu UpdateMenu(
        Guid menuId,
        string name,
        string slug,
        MenuVariant variant,
        bool isMegaMenu,
        int order,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        var menu = _menus.FirstOrDefault(x => x.Id == menuId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Menu not found.");

        EnsureMenuOrderUnique(order, menuId);
        EnsureMenuSlugUnique(slug, menuId);

        menu.Update(name, slug, variant, isMegaMenu, order, isActive, nowUtc);
        UpdatedAtUtc = nowUtc;
        NormalizeMenuOrder(nowUtc);
        return menu;
    }

    public void RemoveMenu(Guid menuId, DateTimeOffset nowUtc)
    {
        var menu = _menus.FirstOrDefault(x => x.Id == menuId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Menu not found.");

        menu.Delete(nowUtc);
        UpdatedAtUtc = nowUtc;
        NormalizeMenuOrder(nowUtc);
    }

    public void Delete(DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
        UpdatedAtUtc = deletedAtUtc;

        foreach (var menu in _menus.Where(x => !x.IsDeleted))
        {
            menu.Delete(deletedAtUtc);
        }
    }

    public void Restore(DateTimeOffset restoredAtUtc)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = restoredAtUtc;
    }

    private void EnsureMenuOrderUnique(int order, Guid? menuId)
    {
        if (_menus.Any(x => !x.IsDeleted && x.Order == order && x.Id != menuId))
        {
            throw new InvalidOperationException("Menu order must be unique per group.");
        }
    }

    private void EnsureMenuSlugUnique(string slug, Guid? menuId)
    {
        var normalized = NormalizeSlug(slug);
        if (_menus.Any(x => !x.IsDeleted && x.Slug == normalized && x.Id != menuId))
        {
            throw new InvalidOperationException("Menu slug must be unique per group.");
        }
    }

    private void NormalizeMenuOrder(DateTimeOffset nowUtc)
    {
        var ordered = _menus.Where(x => !x.IsDeleted).OrderBy(x => x.Order).ThenBy(x => x.CreatedAtUtc).ToList();
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].Reorder(index, nowUtc);
        }
    }

    private static void ValidateName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Name is required.", nameof(value));
        }
    }

    private static void ValidateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Slug is required.", nameof(value));
        }
    }

    private static string NormalizeSlug(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
