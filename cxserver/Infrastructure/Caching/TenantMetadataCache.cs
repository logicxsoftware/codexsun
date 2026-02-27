using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace cxserver.Infrastructure.Caching;

internal sealed class TenantMetadataCache : ITenantMetadataCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeSpan _ttl;

    public TenantMetadataCache(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IOptions<MultiTenancyOptions> multiTenancyOptions)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _ttl = TimeSpan.FromSeconds(Math.Max(5, multiTenancyOptions.Value.Cache.MetadataTtlSeconds));
    }

    public Task<TenantRegistryItem?> GetByDomainAsync(string domain, CancellationToken cancellationToken)
    {
        return GetAsync(TenantCacheKeys.MetadataByDomain(domain), cancellationToken);
    }

    public Task<TenantRegistryItem?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken)
    {
        return GetAsync(TenantCacheKeys.MetadataByIdentifier(identifier), cancellationToken);
    }

    private async Task<TenantRegistryItem?> GetAsync(string key, CancellationToken cancellationToken)
    {

        if (_memoryCache.TryGetValue<TenantRegistryItem>(key, out var cached))
        {
            return cached;
        }

        var bytes = await _distributedCache.GetAsync(key, cancellationToken);

        if (bytes is null || bytes.Length == 0)
        {
            return null;
        }

        var tenant = JsonSerializer.Deserialize<TenantRegistryItem>(bytes, JsonOptions);

        if (tenant is not null)
        {
            _memoryCache.Set(key, tenant, _ttl);
        }

        return tenant;
    }

    public async Task SetAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        var domainKey = TenantCacheKeys.MetadataByDomain(tenant.Domain);
        var identifierKey = TenantCacheKeys.MetadataByIdentifier(tenant.Identifier);

        _memoryCache.Set(domainKey, tenant, _ttl);
        _memoryCache.Set(identifierKey, tenant, _ttl);

        var payload = JsonSerializer.SerializeToUtf8Bytes(tenant, JsonOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _ttl
        };

        await _distributedCache.SetAsync(domainKey, payload, options, cancellationToken);
        await _distributedCache.SetAsync(identifierKey, payload, options, cancellationToken);
    }

    public async Task InvalidateAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        var domainKey = TenantCacheKeys.MetadataByDomain(tenant.Domain);
        var identifierKey = TenantCacheKeys.MetadataByIdentifier(tenant.Identifier);

        _memoryCache.Remove(domainKey);
        _memoryCache.Remove(identifierKey);

        await _distributedCache.RemoveAsync(domainKey, cancellationToken);
        await _distributedCache.RemoveAsync(identifierKey, cancellationToken);
    }
}
