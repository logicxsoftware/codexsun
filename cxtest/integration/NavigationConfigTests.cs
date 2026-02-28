using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using cxtest.TestKit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace cxtest.integration;

public sealed class NavigationConfigTests
{
    [Fact]
    public async Task Navigation_Config_Create_Update_And_Unique_Per_Tenant()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_nav_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("nav-test", "nav-test.localhost", "Nav Test", "tenant_nav_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_nav_db", StringComparison.OrdinalIgnoreCase);
        provider.GetRequiredService<ITenantContextAccessor>().SetTenant(new TenantSession(tenant.TenantId, tenant.Name, "nav-test.localhost", tenantConnection));

        var store = provider.GetRequiredService<IWebsiteNavigationStore>();
        var existing = await store.GetWebNavigationConfigAsync(tenant.TenantId, CancellationToken.None);
        Assert.NotNull(existing);

        var updated = await store.UpsertWebNavigationConfigAsync(
            new UpsertNavigationConfigInput(
                tenant.TenantId,
                JsonDocument.Parse("""{"variant":"full","zoneOrder":["left","center","right"]}"""),
                JsonDocument.Parse("""{"backgroundToken":"header-bg","textToken":"foreground","hoverToken":"primary"}"""),
                JsonDocument.Parse("""{"sticky":true}"""),
                JsonDocument.Parse("""{"left":["logo"],"center":["menu"],"right":["auth"]}"""),
                true),
            CancellationToken.None);

        Assert.Equal(tenant.TenantId, updated.TenantId);
        Assert.True(updated.BehaviorConfig.RootElement.GetProperty("sticky").GetBoolean());
        Assert.DoesNotContain("#", updated.StyleConfig.RootElement.ToString(), StringComparison.Ordinal);
    }
}
