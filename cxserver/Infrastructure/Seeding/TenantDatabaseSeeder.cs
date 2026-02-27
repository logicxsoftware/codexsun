using System.Text.Json;
using cxserver.Domain.Configuration;
using cxserver.Infrastructure.Options;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantDatabaseSeeder
{
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly IOptions<MultiTenancyOptions> _multiTenancyOptions;
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;
    private readonly TenantWebsitePageSeeder _tenantWebsitePageSeeder;

    public TenantDatabaseSeeder(
        ITenantDbContextFactory tenantDbContextFactory,
        IOptions<MultiTenancyOptions> multiTenancyOptions,
        Application.Abstractions.IDateTimeProvider dateTimeProvider,
        TenantWebsitePageSeeder tenantWebsitePageSeeder)
    {
        _tenantDbContextFactory = tenantDbContextFactory;
        _multiTenancyOptions = multiTenancyOptions;
        _dateTimeProvider = dateTimeProvider;
        _tenantWebsitePageSeeder = tenantWebsitePageSeeder;
    }

    public async Task SeedDefaultConfigurationAsync(string connectionString, CancellationToken cancellationToken)
    {
        await using var dbContext = await _tenantDbContextFactory.CreateAsync(connectionString, cancellationToken);

        var defaultTenant = _multiTenancyOptions.Value.DefaultTenant;

        var exists = await dbContext.ConfigurationDocuments.AnyAsync(
            x => x.NamespaceKey == "tenant" && x.DocumentKey == "features",
            cancellationToken);

        if (!exists)
        {
            var payload = JsonDocument.Parse(defaultTenant.FeatureSettingsJson);

            var document = ConfigurationDocument.Create(
                Guid.NewGuid(),
                "tenant",
                "features",
                payload,
                _dateTimeProvider.UtcNow);

            await dbContext.ConfigurationDocuments.AddAsync(document, cancellationToken);
        }

        await _tenantWebsitePageSeeder.SeedAsync(dbContext, cancellationToken);
    }
}
