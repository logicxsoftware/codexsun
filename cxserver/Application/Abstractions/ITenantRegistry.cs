namespace cxserver.Application.Abstractions;

public interface ITenantRegistry
{
    Task<TenantRegistryItem?> GetByDomainAsync(string domain, CancellationToken cancellationToken);
    Task<TenantRegistryItem?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken);
    Task<IReadOnlyList<TenantRegistryItem>> GetActiveAsync(CancellationToken cancellationToken);
    Task<TenantRegistryItem> UpsertAsync(
        string identifier,
        string domain,
        string name,
        string databaseName,
        string connectionString,
        string featureSettingsJson,
        string isolationMetadataJson,
        bool isActive,
        CancellationToken cancellationToken);
    Task<TenantRegistryItem> ActivateAsync(Guid tenantId, CancellationToken cancellationToken);
    Task DeactivateAndDeleteAsync(Guid tenantId, CancellationToken cancellationToken);
}
