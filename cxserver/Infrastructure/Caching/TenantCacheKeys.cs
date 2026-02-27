namespace cxserver.Infrastructure.Caching;

internal static class TenantCacheKeys
{
    public static string Metadata(string identifier) => $"tenant:meta:{identifier.ToLowerInvariant()}";
    public static string Feature(Guid tenantId, string key) => $"tenant:feature:{tenantId:N}:{key.ToLowerInvariant()}";
    public static string FeaturePartition(Guid tenantId) => $"tenant:feature:partition:{tenantId:N}";
}
