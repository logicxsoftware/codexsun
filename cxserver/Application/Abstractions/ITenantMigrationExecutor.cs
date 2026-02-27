namespace cxserver.Application.Abstractions;

public interface ITenantMigrationExecutor
{
    Task ExecuteAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
}
