namespace cxserver.Application.Abstractions;

public interface ITenantOnboardingCoordinator
{
    Task<TenantRegistryItem> ExecuteAsync(TenantRegistryItem tenant, string featureSettingsJson, CancellationToken cancellationToken);
}
