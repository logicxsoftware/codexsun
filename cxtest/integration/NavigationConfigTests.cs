using System.Net.Http.Json;
using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using cxserver.Domain.NavigationEngine;
using cxtest.TestKit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace cxtest.integration;

public sealed class NavigationConfigTests
{
    [Fact]
    public async Task Navigation_Config_Persists_WidthVariant_For_Full_And_Boxed()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_nav_width_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("nav-width-test", "nav-width-test.localhost", "Nav Width Test", "tenant_nav_width_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_nav_width_db", StringComparison.OrdinalIgnoreCase);
        provider.GetRequiredService<ITenantContextAccessor>().SetTenant(new TenantSession(tenant.TenantId, tenant.Name, "nav-width-test.localhost", tenantConnection));

        var store = provider.GetRequiredService<IWebsiteNavigationStore>();
        var full = await store.UpsertWebNavigationConfigAsync(
            new UpsertNavigationConfigInput(
                tenant.TenantId,
                JsonDocument.Parse("""{"variant":"full","zoneOrder":["left","center","right"]}"""),
                JsonDocument.Parse("""{"backgroundToken":"header-bg","textToken":"foreground","hoverToken":"primary"}"""),
                JsonDocument.Parse("""{"sticky":true}"""),
                JsonDocument.Parse("""{"left":["logo"],"center":["menu"],"right":["auth"]}"""),
                true),
            CancellationToken.None);

        var boxed = await store.UpsertWebNavigationConfigAsync(
            new UpsertNavigationConfigInput(
                tenant.TenantId,
                JsonDocument.Parse("""{"variant":"boxed","zoneOrder":["left","center","right"]}"""),
                JsonDocument.Parse("""{"backgroundToken":"header-bg","textToken":"foreground","hoverToken":"primary"}"""),
                JsonDocument.Parse("""{"sticky":true}"""),
                JsonDocument.Parse("""{"left":["logo"],"center":["menu"],"right":["auth"]}"""),
                true),
            CancellationToken.None);

        Assert.Equal(NavWidthVariant.Full, full.WidthVariant);
        Assert.Equal(NavWidthVariant.Boxed, boxed.WidthVariant);
        Assert.Equal(tenant.TenantId, boxed.TenantId);
    }

    [Fact]
    public async Task Navigation_Config_Tenant_Override_And_Global_Fallback_Work()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_nav_scope_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("nav-scope-test", "nav-scope-test.localhost", "Nav Scope Test", "tenant_nav_scope_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_nav_scope_db", StringComparison.OrdinalIgnoreCase);
        provider.GetRequiredService<ITenantContextAccessor>().SetTenant(new TenantSession(tenant.TenantId, tenant.Name, "nav-scope-test.localhost", tenantConnection));

        var store = provider.GetRequiredService<IWebsiteNavigationStore>();

        await store.UpsertWebNavigationConfigAsync(
            new UpsertNavigationConfigInput(
                null,
                JsonDocument.Parse("""{"variant":"full","zoneOrder":["left","center","right"]}"""),
                JsonDocument.Parse("""{"backgroundToken":"header-bg","textToken":"foreground","hoverToken":"primary"}"""),
                JsonDocument.Parse("""{"sticky":true}"""),
                JsonDocument.Parse("""{"left":["logo"],"center":["menu"],"right":["auth"]}"""),
                true),
            CancellationToken.None);

        var fromGlobal = await store.GetWebNavigationConfigAsync(tenant.TenantId, CancellationToken.None);
        Assert.NotNull(fromGlobal);
        Assert.Equal(NavWidthVariant.Full, fromGlobal!.WidthVariant);

        await store.UpsertWebNavigationConfigAsync(
            new UpsertNavigationConfigInput(
                tenant.TenantId,
                JsonDocument.Parse("""{"variant":"boxed","zoneOrder":["left","center","right"]}"""),
                JsonDocument.Parse("""{"backgroundToken":"header-bg","textToken":"foreground","hoverToken":"primary"}"""),
                JsonDocument.Parse("""{"sticky":true}"""),
                JsonDocument.Parse("""{"left":["logo"],"center":["menu"],"right":["auth"]}"""),
                true),
            CancellationToken.None);

        var fromTenant = await store.GetWebNavigationConfigAsync(tenant.TenantId, CancellationToken.None);
        Assert.NotNull(fromTenant);
        Assert.Equal(NavWidthVariant.Boxed, fromTenant!.WidthVariant);
    }

    [Fact]
    public async Task Navigation_Config_Endpoint_Returns_String_Enum_And_Rejects_Invalid_Input()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_nav_api_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboard = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "nav-api-test",
            Domain = "nav-api-test.localhost",
            Name = "Nav Api Test",
            DatabaseName = "tenant_nav_api_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}"
        });
        onboard.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Host = "nav-api-test.localhost";

        var valid = await client.PutAsJsonAsync("/api/admin/web-navigation-config", new
        {
            LayoutConfig = new { variant = "full", zoneOrder = new[] { "left", "center", "right" } },
            StyleConfig = new { backgroundToken = "header-bg", textToken = "foreground", hoverToken = "primary" },
            BehaviorConfig = new { sticky = true },
            ComponentConfig = new { left = new[] { "logo" }, center = new[] { "menu" }, right = new[] { "auth" } },
            IsActive = true
        });
        valid.EnsureSuccessStatusCode();

        var validJson = JsonDocument.Parse(await valid.Content.ReadAsStringAsync());
        Assert.Equal("full", validJson.RootElement.GetProperty("widthVariant").GetString());

        var invalid = await client.PutAsJsonAsync("/api/admin/web-navigation-config", new
        {
            LayoutConfig = new { variant = "wide", zoneOrder = new[] { "left", "center", "right" } },
            StyleConfig = new { backgroundToken = "header-bg", textToken = "foreground", hoverToken = "primary" },
            BehaviorConfig = new { sticky = true },
            ComponentConfig = new { left = new[] { "logo" }, center = new[] { "menu" }, right = new[] { "auth" } },
            IsActive = true
        });

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, invalid.StatusCode);
    }
}
