namespace cxserver.Application.Abstractions;

public interface ITenantDatabaseMigrationService
{
    Task MigrateMasterAsync(CancellationToken cancellationToken);
    Task MigrateTenantAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
    Task MigrateAllTenantsAsync(CancellationToken cancellationToken);
}
