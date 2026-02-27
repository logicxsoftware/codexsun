namespace cxserver.Application.Abstractions;

public interface ITenantDatabaseCreator
{
    Task CreateIfNotExistsAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
    Task DeleteIfExistsAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
}
