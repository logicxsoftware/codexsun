using cxserver.Application.Abstractions;

namespace cxserver.Infrastructure.Tenancy;

internal sealed class DomainTenantLookup : ITenantDomainLookup
{
    private readonly ITenantRegistry _tenantRegistry;

    public DomainTenantLookup(ITenantRegistry tenantRegistry)
    {
        _tenantRegistry = tenantRegistry;
    }

    public Task<TenantRegistryItem?> GetByDomainAsync(string domain, CancellationToken cancellationToken)
    {
        return _tenantRegistry.GetByDomainAsync(domain, cancellationToken);
    }
}
