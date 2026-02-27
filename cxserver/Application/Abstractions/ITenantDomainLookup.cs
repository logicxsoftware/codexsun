namespace cxserver.Application.Abstractions;

public interface ITenantDomainLookup
{
    Task<TenantRegistryItem?> GetByDomainAsync(string domain, CancellationToken cancellationToken);
}
