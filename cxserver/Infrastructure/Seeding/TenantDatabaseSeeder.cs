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
    private readonly TenantAboutPageSeeder _tenantAboutPageSeeder;
    private readonly TenantMenuSeeder _tenantMenuSeeder;
    private readonly TenantNavigationSeeder _tenantNavigationSeeder;
    private readonly TenantSliderSeeder _tenantSliderSeeder;

    public TenantDatabaseSeeder(
        ITenantDbContextFactory tenantDbContextFactory,
        IOptions<MultiTenancyOptions> multiTenancyOptions,
        Application.Abstractions.IDateTimeProvider dateTimeProvider,
        TenantWebsitePageSeeder tenantWebsitePageSeeder,
        TenantAboutPageSeeder tenantAboutPageSeeder,
        TenantMenuSeeder tenantMenuSeeder,
        TenantNavigationSeeder tenantNavigationSeeder,
        TenantSliderSeeder tenantSliderSeeder)
    {
        _tenantDbContextFactory = tenantDbContextFactory;
        _multiTenancyOptions = multiTenancyOptions;
        _dateTimeProvider = dateTimeProvider;
        _tenantWebsitePageSeeder = tenantWebsitePageSeeder;
        _tenantAboutPageSeeder = tenantAboutPageSeeder;
        _tenantMenuSeeder = tenantMenuSeeder;
        _tenantNavigationSeeder = tenantNavigationSeeder;
        _tenantSliderSeeder = tenantSliderSeeder;
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
        await _tenantAboutPageSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantMenuSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantNavigationSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantSliderSeeder.SeedAsync(dbContext, cancellationToken);
    }
}
