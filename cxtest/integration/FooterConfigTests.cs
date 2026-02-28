using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using cxtest.TestKit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace cxtest.integration;

public sealed class FooterConfigTests
{
    [Fact]
    public async Task Footer_Config_Update_Persists_Social_Newsletter_And_Hours()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_footer_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("footer-test", "footer-test.localhost", "Footer Test", "tenant_footer_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_footer_db", StringComparison.OrdinalIgnoreCase);
        provider.GetRequiredService<ITenantContextAccessor>().SetTenant(new TenantSession(tenant.TenantId, tenant.Name, "footer-test.localhost", tenantConnection));

        var store = provider.GetRequiredService<IWebsiteNavigationStore>();
        var updated = await store.UpsertFooterConfigAsync(
            new UpsertNavigationConfigInput(
                tenant.TenantId,
                JsonDocument.Parse("""{"variant":"container","columns":4}"""),
                JsonDocument.Parse("""{"backgroundToken":"footer-bg","textToken":"foreground","linkHoverToken":"primary"}"""),
                JsonDocument.Parse("""{"showNewsletter":true}"""),
                JsonDocument.Parse("""{"social":{"enabled":true,"items":[{"icon":"github","label":"GitHub","url":"https://github.com/codexsun","target":"_blank"}]},"businessHours":{"enabled":true,"items":[{"day":"Monday","hours":"9:00 AM - 6:00 PM"}]},"newsletter":{"enabled":true,"title":"Stay Updated","description":"News"}}"""),
                true),
            CancellationToken.None);

        Assert.Equal(tenant.TenantId, updated.TenantId);
        Assert.True(updated.ComponentConfig.RootElement.GetProperty("social").GetProperty("enabled").GetBoolean());
        Assert.True(updated.BehaviorConfig.RootElement.GetProperty("showNewsletter").GetBoolean());
    }
}
