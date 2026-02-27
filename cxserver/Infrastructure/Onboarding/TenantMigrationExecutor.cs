using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Onboarding;

internal sealed class TenantMigrationExecutor : ITenantMigrationExecutor
{
    private readonly ITenantDbContextFactory _tenantDbContextFactory;

    public TenantMigrationExecutor(ITenantDbContextFactory tenantDbContextFactory)
    {
        _tenantDbContextFactory = tenantDbContextFactory;
    }

    public async Task ExecuteAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        await using var dbContext = await _tenantDbContextFactory.CreateAsync(tenant.ConnectionString, cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
