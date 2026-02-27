namespace cxserver.Application.Abstractions;

public interface ITenantFeatureConfigurationInitializer
{
    Task InitializeAsync(TenantRegistryItem tenant, string featureSettingsJson, CancellationToken cancellationToken);
}
