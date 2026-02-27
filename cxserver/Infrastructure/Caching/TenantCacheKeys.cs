namespace cxserver.Infrastructure.Caching;

internal static class TenantCacheKeys
{
    public static string MetadataByDomain(string domain) => $"tenant:meta:domain:{domain.ToLowerInvariant()}";
    public static string MetadataByIdentifier(string identifier) => $"tenant:meta:identifier:{identifier.ToLowerInvariant()}";
    public static string Feature(Guid tenantId, string key) => $"tenant:feature:{tenantId:N}:{key.ToLowerInvariant()}";
    public static string FeaturePartition(Guid tenantId) => $"tenant:feature:partition:{tenantId:N}";
}
