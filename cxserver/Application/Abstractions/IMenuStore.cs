using cxserver.Domain.MenuEngine;

namespace cxserver.Application.Abstractions;

public interface IMenuStore
{
    Task<IReadOnlyList<MenuGroupItem>> GetMenuGroupsAsync(Guid? tenantId, bool includeGlobal, bool activeOnly, CancellationToken cancellationToken);
    Task<MenuGroupItem?> GetMenuGroupByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<MenuGroupItem> CreateMenuGroupAsync(CreateMenuGroupInput input, CancellationToken cancellationToken);
    Task<MenuGroupItem?> UpdateMenuGroupAsync(UpdateMenuGroupInput input, CancellationToken cancellationToken);
    Task<bool> DeleteMenuGroupAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<MenuItemRecord>> GetMenusByGroupAsync(Guid menuGroupId, bool activeOnly, CancellationToken cancellationToken);
    Task<MenuItemRecord?> GetMenuByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<MenuItemRecord> CreateMenuAsync(CreateMenuInput input, CancellationToken cancellationToken);
    Task<MenuItemRecord?> UpdateMenuAsync(UpdateMenuInput input, CancellationToken cancellationToken);
    Task<bool> DeleteMenuAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<MenuNodeItem>> GetMenuItemTreeAsync(Guid menuId, bool activeOnly, CancellationToken cancellationToken);
    Task<MenuNodeItem> CreateMenuItemAsync(CreateMenuNodeInput input, CancellationToken cancellationToken);
    Task<MenuNodeItem?> UpdateMenuItemAsync(UpdateMenuNodeInput input, CancellationToken cancellationToken);
    Task<bool> DeleteMenuItemAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ReorderMenuItemsAsync(Guid menuId, IReadOnlyList<MenuNodeOrderInput> orders, CancellationToken cancellationToken);

    Task<IReadOnlyList<MenuRenderGroupItem>> GetRenderMenusAsync(Guid? tenantId, bool includeInactive, CancellationToken cancellationToken);
}

public sealed record MenuGroupItem(
    Guid Id,
    Guid? TenantId,
    MenuGroupType Type,
    string Name,
    string Slug,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record MenuItemRecord(
    Guid Id,
    Guid? TenantId,
    Guid MenuGroupId,
    string Name,
    string Slug,
    MenuVariant Variant,
    bool IsMegaMenu,
    int Order,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record MenuNodeItem(
    Guid Id,
    Guid? TenantId,
    Guid MenuId,
    Guid? ParentId,
    string Title,
    string Slug,
    string Url,
    MenuItemTarget Target,
    string? Icon,
    string? Description,
    int Order,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<MenuNodeItem> Children);

public sealed record MenuRenderGroupItem(
    MenuGroupType GroupType,
    string GroupSlug,
    string GroupName,
    IReadOnlyList<MenuRenderItem> Menus);

public sealed record MenuRenderItem(
    string Name,
    string Slug,
    MenuVariant Variant,
    bool IsMegaMenu,
    int Order,
    IReadOnlyList<MenuRenderNodeItem> Items);

public sealed record MenuRenderNodeItem(
    string Title,
    string Slug,
    string Url,
    MenuItemTarget Target,
    string? Icon,
    string? Description,
    int Order,
    IReadOnlyList<MenuRenderNodeItem> Children);

public sealed record CreateMenuGroupInput(
    Guid? TenantId,
    MenuGroupType Type,
    string Name,
    string Slug,
    string? Description,
    bool IsActive);

public sealed record UpdateMenuGroupInput(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive);

public sealed record CreateMenuInput(
    Guid MenuGroupId,
    Guid? TenantId,
    string Name,
    string Slug,
    MenuVariant Variant,
    bool IsMegaMenu,
    int Order,
    bool IsActive);

public sealed record UpdateMenuInput(
    Guid Id,
    string Name,
    string Slug,
    MenuVariant Variant,
    bool IsMegaMenu,
    int Order,
    bool IsActive);

public sealed record CreateMenuNodeInput(
    Guid MenuId,
    Guid? TenantId,
    Guid? ParentId,
    string Title,
    string Slug,
    string Url,
    MenuItemTarget Target,
    string? Icon,
    string? Description,
    int Order,
    bool IsActive);

public sealed record UpdateMenuNodeInput(
    Guid Id,
    Guid? ParentId,
    string Title,
    string Slug,
    string Url,
    MenuItemTarget Target,
    string? Icon,
    string? Description,
    int Order,
    bool IsActive);

public sealed record MenuNodeOrderInput(
    Guid ItemId,
    Guid? ParentId,
    int Order);
