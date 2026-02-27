using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace cxserver.Infrastructure.Caching;

internal sealed class TenantFeatureCache : ITenantFeatureCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeSpan _ttl;

    public TenantFeatureCache(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IOptions<MultiTenancyOptions> multiTenancyOptions)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _ttl = TimeSpan.FromSeconds(Math.Max(5, multiTenancyOptions.Value.Cache.FeatureTtlSeconds));
    }

    public async Task<string?> GetAsync(Guid tenantId, string featureKey, CancellationToken cancellationToken)
    {
        var key = TenantCacheKeys.Feature(tenantId, featureKey);

        if (_memoryCache.TryGetValue<string>(key, out var cached))
        {
            return cached;
        }

        var bytes = await _distributedCache.GetAsync(key, cancellationToken);

        if (bytes is null || bytes.Length == 0)
        {
            return null;
        }

        var payload = JsonSerializer.Deserialize<string>(bytes, JsonOptions);

        if (payload is not null)
        {
            _memoryCache.Set(key, payload, _ttl);
        }

        return payload;
    }

    public async Task SetAsync(Guid tenantId, string featureKey, string payloadJson, CancellationToken cancellationToken)
    {
        var key = TenantCacheKeys.Feature(tenantId, featureKey);
        var partitionKey = TenantCacheKeys.FeaturePartition(tenantId);

        _memoryCache.Set(key, payloadJson, _ttl);

        var bytes = JsonSerializer.SerializeToUtf8Bytes(payloadJson, JsonOptions);
        await _distributedCache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _ttl
        }, cancellationToken);

        var keys = await ReadPartitionAsync(partitionKey, cancellationToken);
        keys.Add(key);
        await WritePartitionAsync(partitionKey, keys, cancellationToken);
    }

    public async Task InvalidateTenantAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var partitionKey = TenantCacheKeys.FeaturePartition(tenantId);
        var keys = await ReadPartitionAsync(partitionKey, cancellationToken);

        foreach (var key in keys)
        {
            _memoryCache.Remove(key);
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        _memoryCache.Remove(partitionKey);
        await _distributedCache.RemoveAsync(partitionKey, cancellationToken);
    }

    private async Task<HashSet<string>> ReadPartitionAsync(string partitionKey, CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<HashSet<string>>(partitionKey, out var localKeys) && localKeys is not null)
        {
            return new HashSet<string>(localKeys, StringComparer.Ordinal);
        }

        var bytes = await _distributedCache.GetAsync(partitionKey, cancellationToken);

        if (bytes is null || bytes.Length == 0)
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        var keys = JsonSerializer.Deserialize<HashSet<string>>(bytes, JsonOptions) ?? new HashSet<string>(StringComparer.Ordinal);
        _memoryCache.Set(partitionKey, keys, _ttl);

        return new HashSet<string>(keys, StringComparer.Ordinal);
    }

    private async Task WritePartitionAsync(string partitionKey, HashSet<string> keys, CancellationToken cancellationToken)
    {
        _memoryCache.Set(partitionKey, new HashSet<string>(keys, StringComparer.Ordinal), _ttl);

        var bytes = JsonSerializer.SerializeToUtf8Bytes(keys, JsonOptions);
        await _distributedCache.SetAsync(partitionKey, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _ttl
        }, cancellationToken);
    }
}
