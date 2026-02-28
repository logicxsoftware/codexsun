using cxserver.Application.Abstractions;
using cxserver.Application.Features.MenuEngine.Commands.MenuGroups;
using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using cxserver.Domain.MenuEngine;
using cxtest.TestKit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace cxtest.integration;

public sealed class TenantTests
{
    [Fact]
    public async Task Create_And_Duplicate_Tenant_Behavior_Is_Correct()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_a_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var first = await sender.Send(new OnboardTenantCommand("tenant-a", "tenant-a.localhost", "Tenant A", "tenant_a_db", "{}", "{}"));
        var second = await sender.Send(new OnboardTenantCommand("tenant-a", "tenant-a.localhost", "Tenant A", "tenant_a_db", "{}", "{}"));

        Assert.False(first.IsExisting);
        Assert.True(second.IsExisting);
        Assert.Equal(first.TenantId, second.TenantId);
    }

    [Fact]
    public async Task Cross_Tenant_Access_Is_Denied()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_alpha_db", "tenant_beta_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var alpha = await sender.Send(new OnboardTenantCommand("alpha", "alpha.localhost", "Alpha", "tenant_alpha_db", "{}", "{}"));
        var beta = await sender.Send(new OnboardTenantCommand("beta", "beta.localhost", "Beta", "tenant_beta_db", "{}", "{}"));
        var alphaConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_alpha_db", StringComparison.OrdinalIgnoreCase);

        var tenantContext = provider.GetRequiredService<ITenantContextAccessor>();
        tenantContext.SetTenant(new TenantSession(alpha.TenantId, "Alpha", "alpha.localhost", alphaConnection));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sender.Send(new CreateMenuGroupCommand(beta.TenantId, MenuGroupType.Header, "H", "h", null, true)));
    }

    [Fact]
    public async Task Deactivate_And_Delete_Removes_From_Active_Registry()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_soft_delete_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var created = await sender.Send(new OnboardTenantCommand("soft-delete", "soft-delete.localhost", "SoftDelete", "tenant_soft_delete_db", "{}", "{}"));

        var registry = provider.GetRequiredService<ITenantRegistry>();
        await registry.DeactivateAndDeleteAsync(created.TenantId, CancellationToken.None);

        var active = await registry.GetActiveAsync(CancellationToken.None);
        Assert.DoesNotContain(active, x => x.TenantId == created.TenantId);
    }
}
