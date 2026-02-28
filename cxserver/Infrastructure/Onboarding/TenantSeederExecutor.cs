using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.Configuration;
using cxserver.Infrastructure.Seeding;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Onboarding;

internal sealed class TenantSeederExecutor : ITenantSeederExecutor
{
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly TenantWebsitePageSeeder _tenantWebsitePageSeeder;
    private readonly TenantAboutPageSeeder _tenantAboutPageSeeder;
    private readonly TenantMenuSeeder _tenantMenuSeeder;
    private readonly TenantNavigationSeeder _tenantNavigationSeeder;
    private readonly TenantSliderSeeder _tenantSliderSeeder;
    private readonly TenantProductCatalogSeeder _tenantProductCatalogSeeder;

    public TenantSeederExecutor(
        ITenantDbContextFactory tenantDbContextFactory,
        IDateTimeProvider dateTimeProvider,
        TenantWebsitePageSeeder tenantWebsitePageSeeder,
        TenantAboutPageSeeder tenantAboutPageSeeder,
        TenantMenuSeeder tenantMenuSeeder,
        TenantNavigationSeeder tenantNavigationSeeder,
        TenantSliderSeeder tenantSliderSeeder,
        TenantProductCatalogSeeder tenantProductCatalogSeeder)
    {
        _tenantDbContextFactory = tenantDbContextFactory;
        _dateTimeProvider = dateTimeProvider;
        _tenantWebsitePageSeeder = tenantWebsitePageSeeder;
        _tenantAboutPageSeeder = tenantAboutPageSeeder;
        _tenantMenuSeeder = tenantMenuSeeder;
        _tenantNavigationSeeder = tenantNavigationSeeder;
        _tenantSliderSeeder = tenantSliderSeeder;
        _tenantProductCatalogSeeder = tenantProductCatalogSeeder;
    }

    public async Task ExecuteAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        await using var dbContext = await _tenantDbContextFactory.CreateAsync(tenant.ConnectionString, cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);

        var exists = await dbContext.ConfigurationDocuments
            .AsTracking()
            .AnyAsync(x => x.NamespaceKey == "tenant" && x.DocumentKey == "bootstrap", cancellationToken);

        if (!exists)
        {
            var payload = JsonDocument.Parse($"{{\"tenantId\":\"{tenant.TenantId:D}\",\"identifier\":\"{tenant.Identifier}\"}}");
            var doc = ConfigurationDocument.Create(Guid.NewGuid(), "tenant", "bootstrap", payload, _dateTimeProvider.UtcNow);
            await dbContext.ConfigurationDocuments.AddAsync(doc, cancellationToken);
        }

        await _tenantWebsitePageSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantAboutPageSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantMenuSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantNavigationSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantSliderSeeder.SeedAsync(dbContext, cancellationToken);
        await _tenantProductCatalogSeeder.SeedAsync(dbContext, tenant.TenantId, cancellationToken);
    }
}
