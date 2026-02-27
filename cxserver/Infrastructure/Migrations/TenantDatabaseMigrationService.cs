using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace cxserver.Infrastructure.Migrations;

internal sealed class TenantDatabaseMigrationService : ITenantDatabaseMigrationService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITenantRegistry _tenantRegistry;
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly MigrationOptions _migrationOptions;

    public TenantDatabaseMigrationService(
        IServiceScopeFactory scopeFactory,
        ITenantRegistry tenantRegistry,
        ITenantDbContextFactory tenantDbContextFactory,
        IOptions<DatabaseOptions> databaseOptions)
    {
        _scopeFactory = scopeFactory;
        _tenantRegistry = tenantRegistry;
        _tenantDbContextFactory = tenantDbContextFactory;
        _migrationOptions = databaseOptions.Value.Migration;
    }

    public async Task MigrateMasterAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var masterDbContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        await masterDbContext.Database.MigrateAsync(cancellationToken);
    }

    public async Task MigrateTenantAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        await using var tenantDbContext = await _tenantDbContextFactory.CreateAsync(tenant.ConnectionString, cancellationToken);
        await tenantDbContext.Database.MigrateAsync(cancellationToken);
    }

    public async Task MigrateAllTenantsAsync(CancellationToken cancellationToken)
    {
        var tenants = await _tenantRegistry.GetActiveAsync(cancellationToken);

        if (tenants.Count == 0)
        {
            return;
        }

        var batchSize = Math.Max(1, _migrationOptions.TenantBatchSize);
        var maxParallel = Math.Max(1, _migrationOptions.MaxParallelTenants);

        for (var index = 0; index < tenants.Count; index += batchSize)
        {
            var batch = tenants.Skip(index).Take(batchSize).ToList();

            using var semaphore = new SemaphoreSlim(maxParallel, maxParallel);
            var tasks = batch.Select(async tenant =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await MigrateTenantAsync(tenant, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
