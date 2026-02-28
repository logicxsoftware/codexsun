using cxserver.Application.Abstractions;
using cxserver.Domain.MenuEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.MenuEngine;

internal sealed class MenuStore : IMenuStore
{
    private readonly ITenantDbContextAccessor _tenantDbContextAccessor;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MenuStore(
        ITenantDbContextAccessor tenantDbContextAccessor,
        ITenantContext tenantContext,
        IDateTimeProvider dateTimeProvider)
    {
        _tenantDbContextAccessor = tenantDbContextAccessor;
        _tenantContext = tenantContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IReadOnlyList<MenuGroupItem>> GetMenuGroupsAsync(Guid? tenantId, bool includeGlobal, bool activeOnly, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var query = dbContext.MenuGroups.AsNoTracking();

        if (tenantId.HasValue)
        {
            query = includeGlobal
                ? query.Where(x => x.TenantId == null || x.TenantId == tenantId)
                : query.Where(x => x.TenantId == tenantId);
        }
        else
        {
            query = query.Where(x => x.TenantId == null);
        }

        if (activeOnly)
        {
            query = query.Where(x => x.IsActive);
        }

        var groups = await query
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return groups.Select(MapGroup).ToList();
    }

    public async Task<MenuGroupItem?> GetMenuGroupByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var group = await dbContext.MenuGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return group is null ? null : MapGroup(group);
    }

    public async Task<MenuGroupItem> CreateMenuGroupAsync(CreateMenuGroupInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var slug = NormalizeSlug(input.Slug);
        var tenantId = ResolveTenantScope(input.TenantId);

        var exists = await dbContext.MenuGroups
            .AsNoTracking()
            .AnyAsync(x => x.TenantId == tenantId && x.Slug == slug, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Menu group slug already exists for the selected tenant scope.");
        }

        var entity = MenuGroup.Create(
            Guid.NewGuid(),
            tenantId,
            input.Type,
            input.Name,
            slug,
            input.Description,
            _dateTimeProvider.UtcNow);

        if (!input.IsActive)
        {
            entity.Update(entity.Name, entity.Slug, entity.Description, false, _dateTimeProvider.UtcNow);
        }

        await dbContext.MenuGroups.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapGroup(entity);
    }

    public async Task<MenuGroupItem?> UpdateMenuGroupAsync(UpdateMenuGroupInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var entity = await dbContext.MenuGroups.AsTracking().FirstOrDefaultAsync(x => x.Id == input.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var normalizedSlug = NormalizeSlug(input.Slug);
        var exists = await dbContext.MenuGroups
            .AsNoTracking()
            .AnyAsync(x => x.Id != input.Id && x.TenantId == entity.TenantId && x.Slug == normalizedSlug, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Menu group slug already exists for the selected tenant scope.");
        }

        entity.Update(input.Name, normalizedSlug, input.Description, input.IsActive, _dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapGroup(entity);
    }

    public async Task<bool> DeleteMenuGroupAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var entity = await dbContext.MenuGroups
            .Include(x => x.Menus)
                .ThenInclude(x => x.Items)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.Delete(_dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<MenuItemRecord>> GetMenusByGroupAsync(Guid menuGroupId, bool activeOnly, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var query = dbContext.Menus.AsNoTracking().Where(x => x.MenuGroupId == menuGroupId);
        if (activeOnly)
        {
            query = query.Where(x => x.IsActive);
        }

        var entities = await query.OrderBy(x => x.Order).ThenBy(x => x.Name).ToListAsync(cancellationToken);
        return entities.Select(MapMenu).ToList();
    }

    public async Task<MenuItemRecord?> GetMenuByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var entity = await dbContext.Menus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null ? null : MapMenu(entity);
    }

    public async Task<MenuItemRecord> CreateMenuAsync(CreateMenuInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var group = await dbContext.MenuGroups
            .Include(x => x.Menus)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == input.MenuGroupId, cancellationToken)
            ?? throw new InvalidOperationException("Menu group not found.");

        var tenantId = ResolveTenantScope(input.TenantId);
        ValidateTenantOwnership(group.TenantId, tenantId);

        var menu = group.AddMenu(
            Guid.NewGuid(),
            tenantId,
            input.Name,
            input.Slug,
            input.Variant,
            input.IsMegaMenu,
            input.Order,
            input.IsActive,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapMenu(menu);
    }

    public async Task<MenuItemRecord?> UpdateMenuAsync(UpdateMenuInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var menu = await dbContext.Menus
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == input.Id, cancellationToken);

        if (menu is null)
        {
            return null;
        }

        var group = await dbContext.MenuGroups
            .Include(x => x.Menus)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == menu.MenuGroupId, cancellationToken)
            ?? throw new InvalidOperationException("Menu group not found.");

        var updated = group.UpdateMenu(
            input.Id,
            input.Name,
            input.Slug,
            input.Variant,
            input.IsMegaMenu,
            input.Order,
            input.IsActive,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapMenu(updated);
    }

    public async Task<bool> DeleteMenuAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var menu = await dbContext.Menus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (menu is null)
        {
            return false;
        }

        var group = await dbContext.MenuGroups
            .Include(x => x.Menus)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == menu.MenuGroupId, cancellationToken)
            ?? throw new InvalidOperationException("Menu group not found.");

        group.RemoveMenu(id, _dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<MenuNodeItem>> GetMenuItemTreeAsync(Guid menuId, bool activeOnly, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var query = dbContext.MenuItems.AsNoTracking().Where(x => x.MenuId == menuId);
        if (activeOnly)
        {
            query = query.Where(x => x.IsActive);
        }

        var items = await query.OrderBy(x => x.Order).ThenBy(x => x.Title).ToListAsync(cancellationToken);
        return BuildTree(items, null);
    }

    public async Task<MenuNodeItem> CreateMenuItemAsync(CreateMenuNodeInput input, CancellationToken cancellationToken)
    {
        ValidateUrl(input.Url);

        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var menu = await dbContext.Menus
            .Include(x => x.Items)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == input.MenuId, cancellationToken)
            ?? throw new InvalidOperationException("Menu not found.");

        var tenantId = ResolveTenantScope(input.TenantId);
        ValidateTenantOwnership(menu.TenantId, tenantId);

        var created = menu.AddItem(
            Guid.NewGuid(),
            tenantId,
            input.ParentId,
            input.Title,
            input.Slug,
            input.Url,
            input.Target,
            input.Icon,
            input.Description,
            input.Order,
            input.IsActive,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new MenuNodeItem(
            created.Id,
            created.TenantId,
            created.MenuId,
            created.ParentId,
            created.Title,
            created.Slug,
            created.Url,
            created.Target,
            created.Icon,
            created.Description,
            created.Order,
            created.IsActive,
            created.CreatedAtUtc,
            created.UpdatedAtUtc,
            []);
    }

    public async Task<MenuNodeItem?> UpdateMenuItemAsync(UpdateMenuNodeInput input, CancellationToken cancellationToken)
    {
        ValidateUrl(input.Url);

        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var item = await dbContext.MenuItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == input.Id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        var menu = await dbContext.Menus
            .Include(x => x.Items)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == item.MenuId, cancellationToken)
            ?? throw new InvalidOperationException("Menu not found.");

        var updated = menu.UpdateItem(
            input.Id,
            input.ParentId,
            input.Title,
            input.Slug,
            input.Url,
            input.Target,
            input.Icon,
            input.Description,
            input.Order,
            input.IsActive,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new MenuNodeItem(
            updated.Id,
            updated.TenantId,
            updated.MenuId,
            updated.ParentId,
            updated.Title,
            updated.Slug,
            updated.Url,
            updated.Target,
            updated.Icon,
            updated.Description,
            updated.Order,
            updated.IsActive,
            updated.CreatedAtUtc,
            updated.UpdatedAtUtc,
            []);
    }

    public async Task<bool> DeleteMenuItemAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var item = await dbContext.MenuItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        var menu = await dbContext.Menus
            .Include(x => x.Items)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == item.MenuId, cancellationToken)
            ?? throw new InvalidOperationException("Menu not found.");

        menu.DeleteItem(id, _dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ReorderMenuItemsAsync(Guid menuId, IReadOnlyList<MenuNodeOrderInput> orders, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var menu = await dbContext.Menus
            .Include(x => x.Items)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == menuId, cancellationToken);

        if (menu is null)
        {
            return false;
        }

        menu.ReorderItems(orders.Select(x => (x.ItemId, x.ParentId, x.Order)).ToList(), _dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<MenuRenderGroupItem>> GetRenderMenusAsync(Guid? tenantId, bool includeInactive, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);

        var groups = await dbContext.MenuGroups
            .AsNoTracking()
            .Where(x => x.TenantId == null || (tenantId.HasValue && x.TenantId == tenantId))
            .ToListAsync(cancellationToken);

        if (!includeInactive)
        {
            groups = groups.Where(x => x.IsActive).ToList();
        }

        var chosenGroups = groups
            .GroupBy(x => x.Slug)
            .Select(x => x.OrderByDescending(g => g.TenantId == tenantId).First())
            .ToList();

        var groupIds = chosenGroups.Select(x => x.Id).ToArray();

        var menus = await dbContext.Menus
            .AsNoTracking()
            .Where(x => groupIds.Contains(x.MenuGroupId) && (x.TenantId == null || (tenantId.HasValue && x.TenantId == tenantId)))
            .ToListAsync(cancellationToken);

        if (!includeInactive)
        {
            menus = menus.Where(x => x.IsActive).ToList();
        }

        var chosenMenus = menus
            .GroupBy(x => new { x.MenuGroupId, x.Slug })
            .Select(x => x.OrderByDescending(m => m.TenantId == tenantId).First())
            .ToList();

        var menuIds = chosenMenus.Select(x => x.Id).ToArray();

        var items = await dbContext.MenuItems
            .AsNoTracking()
            .Where(x => menuIds.Contains(x.MenuId) && (x.TenantId == null || (tenantId.HasValue && x.TenantId == tenantId)))
            .ToListAsync(cancellationToken);

        if (!includeInactive)
        {
            items = items.Where(x => x.IsActive).ToList();
        }

        var chosenItems = items
            .GroupBy(x => new { x.MenuId, x.Slug })
            .Select(x => x.OrderByDescending(i => i.TenantId == tenantId).First())
            .ToList();

        var result = chosenGroups
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .Select(group => new MenuRenderGroupItem(
                group.Type,
                group.Slug,
                group.Name,
                chosenMenus
                    .Where(x => x.MenuGroupId == group.Id)
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.Name)
                    .Select(menu => new MenuRenderItem(
                        menu.Name,
                        menu.Slug,
                        menu.Variant,
                        menu.IsMegaMenu,
                        menu.Order,
                        BuildRenderTree(chosenItems.Where(x => x.MenuId == menu.Id).ToList(), null)))
                    .ToList()))
            .ToList();

        return result;
    }

    private Guid? ResolveTenantScope(Guid? requestedTenantId)
    {
        if (!requestedTenantId.HasValue)
        {
            return null;
        }

        if (!_tenantContext.TenantId.HasValue)
        {
            throw new InvalidOperationException("Tenant scope is not available in current context.");
        }

        if (requestedTenantId != _tenantContext.TenantId)
        {
            throw new InvalidOperationException("Cannot access another tenant scope.");
        }

        return requestedTenantId;
    }

    private static void ValidateTenantOwnership(Guid? ownerTenantId, Guid? childTenantId)
    {
        if (ownerTenantId != childTenantId)
        {
            throw new InvalidOperationException("Tenant scope mismatch.");
        }
    }

    private static IReadOnlyList<MenuNodeItem> BuildTree(List<MenuItem> items, Guid? parentId)
    {
        return items
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Title)
            .Select(x => new MenuNodeItem(
                x.Id,
                x.TenantId,
                x.MenuId,
                x.ParentId,
                x.Title,
                x.Slug,
                x.Url,
                x.Target,
                x.Icon,
                x.Description,
                x.Order,
                x.IsActive,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                BuildTree(items, x.Id)))
            .ToList();
    }

    private static IReadOnlyList<MenuRenderNodeItem> BuildRenderTree(List<MenuItem> items, Guid? parentId)
    {
        return items
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Title)
            .Select(x => new MenuRenderNodeItem(
                x.Title,
                x.Slug,
                x.Url,
                x.Target,
                x.Icon,
                x.Description,
                x.Order,
                BuildRenderTree(items, x.Id)))
            .ToList();
    }

    private static string NormalizeSlug(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static void ValidateUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Url is required.");
        }

        if (value.Contains("\n", StringComparison.Ordinal) || value.Contains("\r", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Url is invalid.");
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var absolute))
        {
            if (absolute.Scheme is not "http" and not "https" and not "mailto")
            {
                throw new InvalidOperationException("Url scheme is not supported.");
            }

            return;
        }

        if (!value.StartsWith("/", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Relative url must start with '/'.");
        }
    }

    private static MenuGroupItem MapGroup(MenuGroup entity)
    {
        return new MenuGroupItem(
            entity.Id,
            entity.TenantId,
            entity.Type,
            entity.Name,
            entity.Slug,
            entity.Description,
            entity.IsActive,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc);
    }

    private static MenuItemRecord MapMenu(Menu entity)
    {
        return new MenuItemRecord(
            entity.Id,
            entity.TenantId,
            entity.MenuGroupId,
            entity.Name,
            entity.Slug,
            entity.Variant,
            entity.IsMegaMenu,
            entity.Order,
            entity.IsActive,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc);
    }
}
