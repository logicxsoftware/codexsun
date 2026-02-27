using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.Tenancy;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Tenancy;

internal sealed class TenantRegistry : ITenantRegistry
{
    private static readonly Func<MasterDbContext, string, IAsyncEnumerable<Tenant>> TenantByDomainQuery =
        EF.CompileAsyncQuery((MasterDbContext db, string domain) =>
            db.Tenants.AsNoTracking().Where(x => x.Domain == domain).Take(1));

    private static readonly Func<MasterDbContext, string, IAsyncEnumerable<Tenant>> TenantByIdentifierQuery =
        EF.CompileAsyncQuery((MasterDbContext db, string identifier) =>
            db.Tenants.AsNoTracking().Where(x => x.Identifier == identifier).Take(1));

    private static readonly Func<MasterDbContext, IAsyncEnumerable<Tenant>> ActiveTenantsQuery =
        EF.CompileAsyncQuery((MasterDbContext db) =>
            db.Tenants.AsNoTracking().Where(x => x.Status == TenantStatus.Active));

    private readonly MasterDbContext _masterDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITenantMetadataCache _tenantMetadataCache;
    private readonly ITenantFeatureCache _tenantFeatureCache;

    public TenantRegistry(
        MasterDbContext masterDbContext,
        IDateTimeProvider dateTimeProvider,
        ITenantMetadataCache tenantMetadataCache,
        ITenantFeatureCache tenantFeatureCache)
    {
        _masterDbContext = masterDbContext;
        _dateTimeProvider = dateTimeProvider;
        _tenantMetadataCache = tenantMetadataCache;
        _tenantFeatureCache = tenantFeatureCache;
    }

    public async Task<TenantRegistryItem?> GetByDomainAsync(string domain, CancellationToken cancellationToken)
    {
        var normalized = domain.Trim();

        var cached = await _tenantMetadataCache.GetByDomainAsync(normalized, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var tenant = await TenantByDomainQuery(_masterDbContext, normalized).FirstOrDefaultAsync(cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        var mapped = Map(tenant);
        await _tenantMetadataCache.SetAsync(mapped, cancellationToken);
        await _tenantFeatureCache.SetAsync(mapped.TenantId, "tenant.features", mapped.FeatureSettingsJson, cancellationToken);

        return mapped;
    }

    public async Task<TenantRegistryItem?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken)
    {
        var normalized = identifier.Trim();

        var cached = await _tenantMetadataCache.GetByIdentifierAsync(normalized, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var tenant = await TenantByIdentifierQuery(_masterDbContext, normalized).FirstOrDefaultAsync(cancellationToken);

        if (tenant is null)
        {
            return null;
        }

        var mapped = Map(tenant);
        await _tenantMetadataCache.SetAsync(mapped, cancellationToken);
        await _tenantFeatureCache.SetAsync(mapped.TenantId, "tenant.features", mapped.FeatureSettingsJson, cancellationToken);

        return mapped;
    }

    public async Task<IReadOnlyList<TenantRegistryItem>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var tenants = await ActiveTenantsQuery(_masterDbContext).ToListAsync(cancellationToken);
        var mapped = tenants.Select(Map).ToList();

        foreach (var item in mapped)
        {
            await _tenantMetadataCache.SetAsync(item, cancellationToken);
            await _tenantFeatureCache.SetAsync(item.TenantId, "tenant.features", item.FeatureSettingsJson, cancellationToken);
        }

        return mapped;
    }

    public async Task<TenantRegistryItem> UpsertAsync(
        string identifier,
        string domain,
        string name,
        string databaseName,
        string connectionString,
        string featureSettingsJson,
        string isolationMetadataJson,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var normalized = identifier.Trim();
        var normalizedDomain = domain.Trim();

        var tenant = await _masterDbContext.Tenants
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Identifier == normalized, cancellationToken);

        var now = _dateTimeProvider.UtcNow;

        if (tenant is null)
        {
            var created = Tenant.Create(
                Guid.NewGuid(),
                normalized,
                normalizedDomain,
                name,
                databaseName,
                connectionString,
                JsonDocument.Parse(featureSettingsJson),
                JsonDocument.Parse(isolationMetadataJson),
                now);

            if (!isActive)
            {
                created.Deactivate(now);
            }

            await _masterDbContext.Tenants.AddAsync(created, cancellationToken);
            await _masterDbContext.SaveChangesAsync(cancellationToken);

            var createdMapped = Map(created);
            await _tenantMetadataCache.SetAsync(createdMapped, cancellationToken);
            await _tenantFeatureCache.SetAsync(createdMapped.TenantId, "tenant.features", createdMapped.FeatureSettingsJson, cancellationToken);
            return createdMapped;
        }

        tenant.UpdateDisplayName(name, now);
        tenant.UpdateDomain(normalizedDomain, now);
        tenant.UpdateConnection(databaseName, connectionString, now);
        tenant.UpdateFeatureSettings(JsonDocument.Parse(featureSettingsJson), now);
        tenant.UpdateIsolationMetadata(JsonDocument.Parse(isolationMetadataJson), now);

        if (isActive)
        {
            tenant.Activate(now);
        }
        else
        {
            tenant.Deactivate(now);
        }

        await _masterDbContext.SaveChangesAsync(cancellationToken);

        var mapped = Map(tenant);
        await _tenantMetadataCache.InvalidateAsync(mapped, cancellationToken);
        await _tenantFeatureCache.InvalidateTenantAsync(mapped.TenantId, cancellationToken);
        await _tenantMetadataCache.SetAsync(mapped, cancellationToken);
        await _tenantFeatureCache.SetAsync(mapped.TenantId, "tenant.features", mapped.FeatureSettingsJson, cancellationToken);

        return mapped;
    }

    public async Task<TenantRegistryItem> ActivateAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenant = await _masterDbContext.Tenants.AsTracking().FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken)
            ?? throw new InvalidOperationException("Tenant not found.");

        tenant.Activate(_dateTimeProvider.UtcNow);
        await _masterDbContext.SaveChangesAsync(cancellationToken);

        var mapped = Map(tenant);
        await _tenantMetadataCache.InvalidateAsync(mapped, cancellationToken);
        await _tenantMetadataCache.SetAsync(mapped, cancellationToken);
        return mapped;
    }

    public async Task DeactivateAndDeleteAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenant = await _masterDbContext.Tenants.AsTracking().FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);

        if (tenant is null)
        {
            return;
        }

        var now = _dateTimeProvider.UtcNow;
        tenant.Deactivate(now);
        tenant.Delete(now);

        await _masterDbContext.SaveChangesAsync(cancellationToken);

        await _tenantMetadataCache.InvalidateAsync(Map(tenant), cancellationToken);
        await _tenantFeatureCache.InvalidateTenantAsync(tenant.Id, cancellationToken);
    }

    private static TenantRegistryItem Map(Tenant tenant)
    {
        return new TenantRegistryItem(
            tenant.Id,
            tenant.Identifier,
            tenant.Domain,
            tenant.Name,
            tenant.ConnectionString,
            tenant.Status == TenantStatus.Active,
            tenant.DatabaseName,
            tenant.FeatureSettings.RootElement.GetRawText(),
            tenant.IsolationMetadata.RootElement.GetRawText());
    }
}
