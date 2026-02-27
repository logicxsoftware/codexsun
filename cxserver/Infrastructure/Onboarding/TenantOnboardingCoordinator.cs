using cxserver.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace cxserver.Infrastructure.Onboarding;

internal sealed class TenantOnboardingCoordinator : ITenantOnboardingCoordinator
{
    private readonly ITenantDatabaseCreator _tenantDatabaseCreator;
    private readonly ITenantMigrationExecutor _tenantMigrationExecutor;
    private readonly ITenantSeederExecutor _tenantSeederExecutor;
    private readonly ITenantFeatureConfigurationInitializer _tenantFeatureConfigurationInitializer;
    private readonly ILogger<TenantOnboardingCoordinator> _logger;

    public TenantOnboardingCoordinator(
        ITenantDatabaseCreator tenantDatabaseCreator,
        ITenantMigrationExecutor tenantMigrationExecutor,
        ITenantSeederExecutor tenantSeederExecutor,
        ITenantFeatureConfigurationInitializer tenantFeatureConfigurationInitializer,
        ILogger<TenantOnboardingCoordinator> logger)
    {
        _tenantDatabaseCreator = tenantDatabaseCreator;
        _tenantMigrationExecutor = tenantMigrationExecutor;
        _tenantSeederExecutor = tenantSeederExecutor;
        _tenantFeatureConfigurationInitializer = tenantFeatureConfigurationInitializer;
        _logger = logger;
    }

    public async Task<TenantRegistryItem> ExecuteAsync(TenantRegistryItem tenant, string featureSettingsJson, CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteWithRetryAsync(() => _tenantDatabaseCreator.CreateIfNotExistsAsync(tenant, cancellationToken), cancellationToken);
            await ExecuteWithRetryAsync(() => _tenantMigrationExecutor.ExecuteAsync(tenant, cancellationToken), cancellationToken);
            await ExecuteWithRetryAsync(() => _tenantSeederExecutor.ExecuteAsync(tenant, cancellationToken), cancellationToken);
            await ExecuteWithRetryAsync(() => _tenantFeatureConfigurationInitializer.InitializeAsync(tenant, featureSettingsJson, cancellationToken), cancellationToken);

            return tenant;
        }
        catch
        {
            try
            {
                await ExecuteWithRetryAsync(() => _tenantDatabaseCreator.DeleteIfExistsAsync(tenant, cancellationToken), cancellationToken);
            }
            catch (Exception cleanupEx)
            {
                _logger.LogError(cleanupEx, "Failed compensation cleanup for tenant {TenantIdentifier}", tenant.Identifier);
            }

            throw;
        }
    }

    private static async Task ExecuteWithRetryAsync(Func<Task> operation, CancellationToken cancellationToken)
    {
        var delay = TimeSpan.FromMilliseconds(200);

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await operation();
                return;
            }
            catch when (attempt < 3)
            {
                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
            }
        }
    }
}
