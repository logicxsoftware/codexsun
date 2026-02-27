using cxserver.Application.Abstractions;

namespace cxserver.Infrastructure.Tenancy;

internal sealed class TenantResolver : ITenantResolver
{
    private readonly ITenantRegistry _tenantRegistry;

    public TenantResolver(ITenantRegistry tenantRegistry)
    {
        _tenantRegistry = tenantRegistry;
    }

    public async Task<TenantRegistryItem?> ResolveAsync(string identifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return null;
        }

        var tenant = await _tenantRegistry.GetByIdentifierAsync(identifier.Trim(), cancellationToken);

        if (tenant is null || !tenant.IsActive)
        {
            return null;
        }

        return tenant;
    }
}
