namespace cxserver.Application.Abstractions;

public interface ITenantMetadataCache
{
    Task<TenantRegistryItem?> GetByDomainAsync(string domain, CancellationToken cancellationToken);
    Task<TenantRegistryItem?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken);
    Task SetAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
    Task InvalidateAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
}
