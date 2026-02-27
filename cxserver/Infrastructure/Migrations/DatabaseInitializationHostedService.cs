using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Seeding;

namespace cxserver.Infrastructure.Migrations;

internal sealed class DatabaseInitializationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseInitializationHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var migrationService = scope.ServiceProvider.GetRequiredService<ITenantDatabaseMigrationService>();
        var masterDatabaseSeeder = scope.ServiceProvider.GetRequiredService<MasterDatabaseSeeder>();
        var tenantDatabaseSeeder = scope.ServiceProvider.GetRequiredService<TenantDatabaseSeeder>();
        var tenantRegistry = scope.ServiceProvider.GetRequiredService<ITenantRegistry>();

        await migrationService.MigrateMasterAsync(cancellationToken);

        await masterDatabaseSeeder.SeedDefaultTenantAsync(cancellationToken);

        await migrationService.MigrateAllTenantsAsync(cancellationToken);

        var activeTenants = await tenantRegistry.GetActiveAsync(cancellationToken);

        foreach (var tenant in activeTenants)
        {
            await tenantDatabaseSeeder.SeedDefaultConfigurationAsync(tenant.ConnectionString, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
