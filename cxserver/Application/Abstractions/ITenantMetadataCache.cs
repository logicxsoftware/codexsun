namespace cxserver.Application.Abstractions;

public interface ITenantMetadataCache
{
    Task<TenantRegistryItem?> GetAsync(string identifier, CancellationToken cancellationToken);
    Task SetAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
    Task InvalidateAsync(string identifier, CancellationToken cancellationToken);
}
