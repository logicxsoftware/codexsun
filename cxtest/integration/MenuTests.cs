using cxserver.Application.Abstractions;
using cxserver.Application.Features.MenuEngine.Commands.MenuGroups;
using cxserver.Application.Features.MenuEngine.Commands.MenuItems;
using cxserver.Application.Features.MenuEngine.Commands.Menus;
using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using cxserver.Domain.MenuEngine;
using cxtest.TestKit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace cxtest.integration;

public sealed class MenuTests
{
    [Fact]
    public async Task Menu_Group_Menu_And_Items_CRUD_Works()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_menu_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("menu-test", "menu-test.localhost", "Menu Test", "tenant_menu_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_menu_db", StringComparison.OrdinalIgnoreCase);

        var tenantContext = provider.GetRequiredService<ITenantContextAccessor>();
        tenantContext.SetTenant(new TenantSession(tenant.TenantId, "Menu Test", "menu-test.localhost", tenantConnection));

        var group = await sender.Send(new CreateMenuGroupCommand(tenant.TenantId, MenuGroupType.Header, "Header", "header-main", "Header", true));
        var menu = await sender.Send(new CreateMenuCommand(group.Id, tenant.TenantId, "Primary", "primary", MenuVariant.Custom, true, 0, true));
        var parent = await sender.Send(new CreateMenuItemCommand(menu.Id, tenant.TenantId, null, "Products", "products", "/products", MenuItemTarget.Self, null, null, 0, true));
        var child = await sender.Send(new CreateMenuItemCommand(menu.Id, tenant.TenantId, parent.Id, "CRM", "crm", "/products/crm", MenuItemTarget.Self, null, null, 0, true));

        var reordered = await sender.Send(new ReorderMenuItemsCommand(menu.Id, [new ReorderMenuItemsEntry(child.Id, null, 0), new ReorderMenuItemsEntry(parent.Id, null, 1)]));
        Assert.True(reordered);
    }

    [Fact]
    public async Task Duplicate_Slug_And_Circular_Parent_Are_Prevented()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_menu2_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("menu-test-2", "menu-test-2.localhost", "Menu Test 2", "tenant_menu2_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_menu2_db", StringComparison.OrdinalIgnoreCase);
        var tenantContext = provider.GetRequiredService<ITenantContextAccessor>();
        tenantContext.SetTenant(new TenantSession(tenant.TenantId, "Menu Test 2", "menu-test-2.localhost", tenantConnection));

        var group = await sender.Send(new CreateMenuGroupCommand(tenant.TenantId, MenuGroupType.Header, "Header", "header-main", "Header", true));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sender.Send(new CreateMenuGroupCommand(tenant.TenantId, MenuGroupType.Header, "Header2", "header-main", "Header", true)));

        var menu = await sender.Send(new CreateMenuCommand(group.Id, tenant.TenantId, "Primary", "primary", MenuVariant.Custom, false, 0, true));
        var root = await sender.Send(new CreateMenuItemCommand(menu.Id, tenant.TenantId, null, "Root", "root", "/root", MenuItemTarget.Self, null, null, 0, true));
        var child = await sender.Send(new CreateMenuItemCommand(menu.Id, tenant.TenantId, root.Id, "Child", "child", "/child", MenuItemTarget.Self, null, null, 0, true));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sender.Send(new UpdateMenuItemCommand(root.Id, child.Id, "Root", "root", "/root", MenuItemTarget.Self, null, null, 0, true)));
    }
}
