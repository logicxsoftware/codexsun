using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace cxserver.Infrastructure.Seeding;

internal sealed class MasterDatabaseSeeder
{
    private readonly ITenantRegistry _tenantRegistry;
    private readonly IOptions<MultiTenancyOptions> _multiTenancyOptions;
    private readonly ITenantConnectionStringBuilder _tenantConnectionStringBuilder;

    public MasterDatabaseSeeder(
        ITenantRegistry tenantRegistry,
        IOptions<MultiTenancyOptions> multiTenancyOptions,
        ITenantConnectionStringBuilder tenantConnectionStringBuilder)
    {
        _tenantRegistry = tenantRegistry;
        _multiTenancyOptions = multiTenancyOptions;
        _tenantConnectionStringBuilder = tenantConnectionStringBuilder;
    }

    public async Task<TenantRegistryItem> SeedDefaultTenantAsync(CancellationToken cancellationToken)
    {
        var defaultTenant = _multiTenancyOptions.Value.DefaultTenant;

        var connectionString = await _tenantConnectionStringBuilder.BuildAsync(defaultTenant.DatabaseName, cancellationToken);

        return await _tenantRegistry.UpsertAsync(
            defaultTenant.Identifier,
            defaultTenant.Domain,
            defaultTenant.Name,
            defaultTenant.DatabaseName,
            connectionString,
            defaultTenant.FeatureSettingsJson,
            defaultTenant.IsolationMetadataJson,
            true,
            cancellationToken);
    }
}
