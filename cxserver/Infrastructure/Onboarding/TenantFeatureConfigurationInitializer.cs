using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.Configuration;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Onboarding;

internal sealed class TenantFeatureConfigurationInitializer : ITenantFeatureConfigurationInitializer
{
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITenantFeatureCache _tenantFeatureCache;

    public TenantFeatureConfigurationInitializer(
        ITenantDbContextFactory tenantDbContextFactory,
        IDateTimeProvider dateTimeProvider,
        ITenantFeatureCache tenantFeatureCache)
    {
        _tenantDbContextFactory = tenantDbContextFactory;
        _dateTimeProvider = dateTimeProvider;
        _tenantFeatureCache = tenantFeatureCache;
    }

    public async Task InitializeAsync(TenantRegistryItem tenant, string featureSettingsJson, CancellationToken cancellationToken)
    {
        await using var dbContext = await _tenantDbContextFactory.CreateAsync(tenant.ConnectionString, cancellationToken);

        var existing = await dbContext.ConfigurationDocuments
            .AsTracking()
            .FirstOrDefaultAsync(x => x.NamespaceKey == "tenant" && x.DocumentKey == "features", cancellationToken);

        var payload = JsonDocument.Parse(featureSettingsJson);

        if (existing is null)
        {
            var created = ConfigurationDocument.Create(Guid.NewGuid(), "tenant", "features", payload, _dateTimeProvider.UtcNow);
            await dbContext.ConfigurationDocuments.AddAsync(created, cancellationToken);
        }
        else
        {
            existing.UpdatePayload(payload, _dateTimeProvider.UtcNow);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await _tenantFeatureCache.SetAsync(tenant.TenantId, "tenant.features", payload.RootElement.GetRawText(), cancellationToken);
    }
}
