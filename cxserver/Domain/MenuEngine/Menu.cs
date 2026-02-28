using cxserver.Domain.Common;

namespace cxserver.Domain.MenuEngine;

public sealed class Menu : ISoftDeletable
{
    private readonly List<MenuItem> _items;

    private Menu(
        Guid id,
        Guid? tenantId,
        Guid menuGroupId,
        string name,
        string slug,
        MenuVariant variant,
        bool isMegaMenu,
        int order,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        Id = id;
        TenantId = tenantId;
        MenuGroupId = menuGroupId;
        Name = name;
        Slug = slug;
        Variant = variant;
        IsMegaMenu = isMegaMenu;
        Order = order;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
        _items = new List<MenuItem>();
    }

    private Menu()
    {
        Name = string.Empty;
        Slug = string.Empty;
        _items = new List<MenuItem>();
    }

    public Guid Id { get; private set; }
    public Guid? TenantId { get; private set; }
    public Guid MenuGroupId { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public MenuVariant Variant { get; private set; }
    public bool IsMegaMenu { get; private set; }
    public int Order { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public IReadOnlyCollection<MenuItem> Items => _items.AsReadOnly();

    internal static Menu Create(
        Guid id,
        Guid? tenantId,
        Guid menuGroupId,
        string name,
        string slug,
        MenuVariant variant,
        bool isMegaMenu,
        int order,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        ValidateName(name);
        ValidateSlug(slug);
        ValidateOrder(order);

        return new Menu(
            id,
            tenantId,
            menuGroupId,
            name.Trim(),
            NormalizeSlug(slug),
            variant,
            isMegaMenu,
            order,
            isActive,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    internal void Update(string name, string slug, MenuVariant variant, bool isMegaMenu, int order, bool isActive, DateTimeOffset nowUtc)
    {
        ValidateName(name);
        ValidateSlug(slug);
        ValidateOrder(order);

        Name = name.Trim();
        Slug = NormalizeSlug(slug);
        Variant = variant;
        IsMegaMenu = isMegaMenu;
        Order = order;
        IsActive = isActive;
        UpdatedAtUtc = nowUtc;
    }

    internal void Reorder(int order, DateTimeOffset nowUtc)
    {
        ValidateOrder(order);
        Order = order;
        UpdatedAtUtc = nowUtc;
    }

    public MenuItem AddItem(
        Guid itemId,
        Guid? tenantId,
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
        EnsureParentExists(parentId);
        EnsureItemOrderUnique(parentId, order, null);
        EnsureItemSlugUnique(slug, null);

        var item = MenuItem.Create(itemId, tenantId, Id, parentId, title, slug, url, target, icon, description, order, isActive, nowUtc);
        _items.Add(item);
        UpdatedAtUtc = nowUtc;
        NormalizeItemOrder(parentId, nowUtc);
        return item;
    }

    public MenuItem UpdateItem(
        Guid itemId,
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
        var item = _items.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Menu item not found.");

        EnsureParentExists(parentId);
        EnsureNoCircularReference(itemId, parentId);
        EnsureItemOrderUnique(parentId, order, itemId);
        EnsureItemSlugUnique(slug, itemId);

        var previousParentId = item.ParentId;
        item.Update(parentId, title, slug, url, target, icon, description, order, isActive, nowUtc);

        if (previousParentId != parentId)
        {
            NormalizeItemOrder(previousParentId, nowUtc);
        }

        UpdatedAtUtc = nowUtc;
        NormalizeItemOrder(parentId, nowUtc);
        return item;
    }

    public void DeleteItem(Guid itemId, DateTimeOffset nowUtc)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Menu item not found.");

        DeleteItemBranch(item.Id, nowUtc);
        UpdatedAtUtc = nowUtc;
        NormalizeItemOrder(item.ParentId, nowUtc);
    }

    public void ReorderItems(IReadOnlyList<(Guid ItemId, Guid? ParentId, int Order)> itemOrder, DateTimeOffset nowUtc)
    {
        if (itemOrder.Count == 0)
        {
            return;
        }

        var activeItems = _items.Where(x => !x.IsDeleted).ToDictionary(x => x.Id);
        var uniqueIds = itemOrder.Select(x => x.ItemId).Distinct().Count();
        if (uniqueIds != itemOrder.Count)
        {
            throw new InvalidOperationException("Duplicate menu item ids in reorder request.");
        }

        foreach (var tuple in itemOrder)
        {
            if (!activeItems.TryGetValue(tuple.ItemId, out var item))
            {
                throw new InvalidOperationException("Invalid menu item id in reorder request.");
            }

            if (tuple.ParentId.HasValue && !activeItems.ContainsKey(tuple.ParentId.Value))
            {
                throw new InvalidOperationException("Invalid parent id in reorder request.");
            }

            EnsureNoCircularReference(tuple.ItemId, tuple.ParentId);
            item.Reorder(tuple.ParentId, tuple.Order, nowUtc);
        }

        UpdatedAtUtc = nowUtc;

        var parentGroups = _items.Where(x => !x.IsDeleted).Select(x => x.ParentId).Distinct().ToList();
        foreach (var parentId in parentGroups)
        {
            NormalizeItemOrder(parentId, nowUtc);
        }
    }

    public void Delete(DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
        UpdatedAtUtc = deletedAtUtc;

        foreach (var item in _items.Where(x => !x.IsDeleted))
        {
            item.Delete(deletedAtUtc);
        }
    }

    public void Restore(DateTimeOffset restoredAtUtc)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = restoredAtUtc;
    }

    private void DeleteItemBranch(Guid rootId, DateTimeOffset nowUtc)
    {
        var root = _items.First(x => x.Id == rootId);
        root.Delete(nowUtc);

        var children = _items.Where(x => !x.IsDeleted && x.ParentId == rootId).Select(x => x.Id).ToList();
        foreach (var childId in children)
        {
            DeleteItemBranch(childId, nowUtc);
        }
    }

    private void EnsureNoCircularReference(Guid itemId, Guid? parentId)
    {
        if (!parentId.HasValue)
        {
            return;
        }

        if (parentId == itemId)
        {
            throw new InvalidOperationException("Menu item parent cannot reference itself.");
        }

        var cursor = parentId;
        while (cursor.HasValue)
        {
            if (cursor.Value == itemId)
            {
                throw new InvalidOperationException("Menu item parent relationship is circular.");
            }

            var parent = _items.FirstOrDefault(x => x.Id == cursor.Value && !x.IsDeleted);
            cursor = parent?.ParentId;
        }
    }

    private void EnsureParentExists(Guid? parentId)
    {
        if (!parentId.HasValue)
        {
            return;
        }

        if (_items.All(x => x.Id != parentId.Value || x.IsDeleted))
        {
            throw new InvalidOperationException("Menu item parent not found.");
        }
    }

    private void EnsureItemOrderUnique(Guid? parentId, int order, Guid? itemId)
    {
        if (_items.Any(x => !x.IsDeleted && x.ParentId == parentId && x.Order == order && x.Id != itemId))
        {
            throw new InvalidOperationException("Menu item order must be unique per parent.");
        }
    }

    private void EnsureItemSlugUnique(string slug, Guid? itemId)
    {
        var normalized = NormalizeSlug(slug);
        if (_items.Any(x => !x.IsDeleted && x.Slug == normalized && x.Id != itemId))
        {
            throw new InvalidOperationException("Menu item slug must be unique per menu.");
        }
    }

    private void NormalizeItemOrder(Guid? parentId, DateTimeOffset nowUtc)
    {
        var siblings = _items
            .Where(x => !x.IsDeleted && x.ParentId == parentId)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.CreatedAtUtc)
            .ToList();

        for (var index = 0; index < siblings.Count; index++)
        {
            siblings[index].Reorder(parentId, index, nowUtc);
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
}
