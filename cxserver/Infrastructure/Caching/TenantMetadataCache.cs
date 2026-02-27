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

    public async Task<TenantRegistryItem?> GetAsync(string identifier, CancellationToken cancellationToken)
    {
        var key = TenantCacheKeys.Metadata(identifier);

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
        var key = TenantCacheKeys.Metadata(tenant.Identifier);
        _memoryCache.Set(key, tenant, _ttl);

        var payload = JsonSerializer.SerializeToUtf8Bytes(tenant, JsonOptions);
        await _distributedCache.SetAsync(key, payload, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _ttl
        }, cancellationToken);
    }

    public async Task InvalidateAsync(string identifier, CancellationToken cancellationToken)
    {
        var key = TenantCacheKeys.Metadata(identifier);
        _memoryCache.Remove(key);
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }
}
