namespace cxserver.Application.Abstractions;

public interface ITenantFeatureCache
{
    Task<string?> GetAsync(Guid tenantId, string featureKey, CancellationToken cancellationToken);
    Task SetAsync(Guid tenantId, string featureKey, string payloadJson, CancellationToken cancellationToken);
    Task InvalidateTenantAsync(Guid tenantId, CancellationToken cancellationToken);
}
