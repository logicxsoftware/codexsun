using cxserver.Domain.MenuEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantMenuSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantMenuSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var groups = await dbContext.MenuGroups
            .Include(x => x.Menus)
                .ThenInclude(x => x.Items)
            .Where(x => x.TenantId == null)
            .AsTracking()
            .ToListAsync(cancellationToken);

        var headerGroup = EnsureGroup(groups, MenuGroupType.Header, "Header", "header", "Codexsun header navigation", now);
        var footerGroup = EnsureGroup(groups, MenuGroupType.Footer, "Footer", "footer", "Codexsun footer navigation", now);
        var mobileGroup = EnsureGroup(groups, MenuGroupType.Mobile, "Mobile", "mobile", "Codexsun mobile navigation", now);
        var sideGroup = EnsureGroup(groups, MenuGroupType.SideMenu, "Sidemenu", "sidemenu", "Codexsun side navigation", now);

        EnsureHeader(headerGroup, now);
        EnsureMobile(mobileGroup, now);
        EnsureFooter(footerGroup, now);
        EnsureSideMenu(sideGroup, now);

        if (groups.Any(x => dbContext.Entry(x).State == EntityState.Detached))
        {
            await dbContext.MenuGroups.AddRangeAsync(groups.Where(x => dbContext.Entry(x).State == EntityState.Detached), cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static MenuGroup EnsureGroup(
        List<MenuGroup> groups,
        MenuGroupType type,
        string name,
        string slug,
        string description,
        DateTimeOffset now)
    {
        var group = groups.FirstOrDefault(x => x.Slug == slug && x.Type == type);
        if (group is null)
        {
            group = MenuGroup.Create(Guid.NewGuid(), null, type, name, slug, description, now);
            groups.Add(group);
            return group;
        }

        group.Update(name, slug, description, true, now);
        return group;
    }

    private static void EnsureHeader(MenuGroup group, DateTimeOffset now)
    {
        var menu = EnsureMenu(group, "Primary", "primary", MenuVariant.Custom, true, 0, now);

        PrepareTopLevelMenu(
            menu,
            ["home", "shop", "services-main", "company-main", "blog-main"],
            now);

        var home = EnsureMenuItem(menu, null, "Home", "home", "/", 0, null, now);
        var shop = EnsureMenuItem(menu, null, "Shop", "shop", "/products", 1, null, now);
        var services = EnsureMenuItem(menu, null, "Services", "services-main", "/services", 2, null, now);
        var company = EnsureMenuItem(menu, null, "Company", "company-main", "/company", 3, null, now);
        var blog = EnsureMenuItem(menu, null, "Blog", "blog-main", "/blog", 4, null, now);
        _ = home;
        _ = shop;
        _ = services;
        _ = company;
        _ = blog;
    }

    private static void EnsureMobile(MenuGroup group, DateTimeOffset now)
    {
        var menu = EnsureMenu(group, "Mobile Primary", "mobile-primary", MenuVariant.Custom, false, 0, now);

        PrepareTopLevelMenu(
            menu,
            ["home-mobile", "shop-mobile", "services-main-mobile", "company-main-mobile", "blog-main-mobile"],
            now);

        var home = EnsureMenuItem(menu, null, "Home", "home-mobile", "/", 0, null, now);
        var shop = EnsureMenuItem(menu, null, "Shop", "shop-mobile", "/products", 1, null, now);
        var services = EnsureMenuItem(menu, null, "Services", "services-main-mobile", "/services", 2, null, now);
        var company = EnsureMenuItem(menu, null, "Company", "company-main-mobile", "/company", 3, null, now);
        var blog = EnsureMenuItem(menu, null, "Blog", "blog-main-mobile", "/blog", 4, null, now);
        _ = home;
        _ = shop;
        _ = services;
        _ = company;
        _ = blog;
    }

    private static void EnsureFooter(MenuGroup group, DateTimeOffset now)
    {
        var product = EnsureMenu(group, "Product", "footer-product", MenuVariant.Custom, false, 0, now);
        var company = EnsureMenu(group, "Company", "footer-company", MenuVariant.Custom, false, 1, now);
        var support = EnsureMenu(group, "Support", "footer-support", MenuVariant.Custom, false, 2, now);
        var legal = EnsureMenu(group, "Legal", "footer-legal", MenuVariant.Custom, false, 3, now);

        EnsureMenuItem(product, null, "CRM", "crm", "/products/crm", 0, null, now);
        EnsureMenuItem(product, null, "ERP", "erp", "/products/erp", 1, null, now);
        EnsureMenuItem(product, null, "HRMS", "hrms", "/products/hrms", 2, null, now);
        EnsureMenuItem(product, null, "POS", "pos", "/products/pos", 3, null, now);
        EnsureMenuItem(product, null, "Custom SaaS", "custom-saas", "/products/custom-saas", 4, null, now);

        EnsureMenuItem(company, null, "About", "about", "/about", 0, null, now);
        EnsureMenuItem(company, null, "Careers", "careers", "/careers", 1, null, now);
        EnsureMenuItem(company, null, "Blog", "blog", "/blog", 2, null, now);
        EnsureMenuItem(company, null, "Contact", "contact", "/contact", 3, null, now);

        EnsureMenuItem(support, null, "Help Center", "help-center", "/help", 0, null, now);
        EnsureMenuItem(support, null, "Documentation", "documentation", "/docs", 1, null, now);
        EnsureMenuItem(support, null, "Status", "status", "/status", 2, null, now);
        EnsureMenuItem(support, null, "Privacy Policy", "privacy-policy", "/privacy-policy", 3, null, now);
        EnsureMenuItem(support, null, "Terms of Service", "terms-of-service", "/terms", 4, null, now);

        EnsureMenuItem(legal, null, "Privacy Policy", "privacy-policy-legal", "/privacy-policy", 0, null, now);
        EnsureMenuItem(legal, null, "Terms", "terms-legal", "/terms", 1, null, now);
        EnsureMenuItem(legal, null, "Cookie Policy", "cookie-policy", "/cookies", 2, null, now);
        EnsureMenuItem(legal, null, "Refund Policy", "refund-policy", "/refund-policy", 3, null, now);
    }

    private static void EnsureSideMenu(MenuGroup group, DateTimeOffset now)
    {
        var menu = EnsureMenu(group, "Application", "app", MenuVariant.Custom, false, 0, now);
        EnsureMenuItem(menu, null, "Dashboard", "dashboard", "/app", 0, null, now);
        EnsureMenuItem(menu, null, "Menu Groups", "menu-groups", "/admin/menu-groups", 1, null, now);
        EnsureMenuItem(menu, null, "Web Menu Builder", "web-menu-builder", "/admin/web-menu-builder", 2, null, now);
    }

    private static Menu EnsureMenu(
        MenuGroup group,
        string name,
        string slug,
        MenuVariant variant,
        bool isMegaMenu,
        int order,
        DateTimeOffset now)
    {
        var menu = group.Menus.FirstOrDefault(x => x.Slug == slug && !x.IsDeleted);
        if (menu is null)
        {
            return group.AddMenu(Guid.NewGuid(), null, name, slug, variant, isMegaMenu, order, true, now);
        }

        return group.UpdateMenu(menu.Id, name, slug, variant, isMegaMenu, order, true, now);
    }

    private static MenuItem EnsureMenuItem(
        Menu menu,
        Guid? parentId,
        string title,
        string slug,
        string url,
        int order,
        string? description,
        DateTimeOffset now)
    {
        var item = menu.Items.FirstOrDefault(x => x.Slug == slug && !x.IsDeleted);
        if (item is null)
        {
            return menu.AddItem(Guid.NewGuid(), null, parentId, title, slug, url, MenuItemTarget.Self, null, description, order, true, now);
        }

        return menu.UpdateItem(item.Id, parentId, title, slug, url, MenuItemTarget.Self, null, description, order, true, now);
    }

    private static void RemoveMenuItemBySlug(Menu menu, string slug, DateTimeOffset now)
    {
        var item = menu.Items.FirstOrDefault(x => x.Slug == slug && !x.IsDeleted);
        if (item is null)
        {
            return;
        }

        menu.DeleteItem(item.Id, now);
    }

    private static void PrepareTopLevelMenu(Menu menu, IReadOnlyCollection<string> requiredSlugs, DateTimeOffset now)
    {
        var normalizedRequired = requiredSlugs
            .Select(x => x.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.Ordinal);

        var topLevelItems = menu.Items
            .Where(x => !x.IsDeleted && x.ParentId == null)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.CreatedAtUtc)
            .ToList();

        foreach (var item in topLevelItems)
        {
            if (!normalizedRequired.Contains(item.Slug))
            {
                menu.DeleteItem(item.Id, now);
            }
        }

        var existingRequired = menu.Items
            .Where(x => !x.IsDeleted && x.ParentId == null && normalizedRequired.Contains(x.Slug))
            .OrderBy(x => x.Order)
            .ThenBy(x => x.CreatedAtUtc)
            .ToList();

        for (var index = 0; index < existingRequired.Count; index++)
        {
            var item = existingRequired[index];
            menu.UpdateItem(item.Id, null, item.Title, item.Slug, item.Url, item.Target, item.Icon, item.Description, 100 + index, item.IsActive, now);
        }
    }
}
