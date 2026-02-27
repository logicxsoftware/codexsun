using cxserver.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace cxserver.Application.Features.Tenants.Commands.OnboardTenant;

internal sealed class OnboardTenantCommandHandler : IRequestHandler<OnboardTenantCommand, OnboardTenantResult>
{
    private readonly ITenantRegistry _tenantRegistry;
    private readonly ITenantConnectionStringBuilder _tenantConnectionStringBuilder;
    private readonly ITenantOnboardingCoordinator _tenantOnboardingCoordinator;
    private readonly ILogger<OnboardTenantCommandHandler> _logger;

    public OnboardTenantCommandHandler(
        ITenantRegistry tenantRegistry,
        ITenantConnectionStringBuilder tenantConnectionStringBuilder,
        ITenantOnboardingCoordinator tenantOnboardingCoordinator,
        ILogger<OnboardTenantCommandHandler> logger)
    {
        _tenantRegistry = tenantRegistry;
        _tenantConnectionStringBuilder = tenantConnectionStringBuilder;
        _tenantOnboardingCoordinator = tenantOnboardingCoordinator;
        _logger = logger;
    }

    public async Task<OnboardTenantResult> Handle(OnboardTenantCommand request, CancellationToken cancellationToken)
    {
        var normalizedIdentifier = request.Identifier.Trim();
        var normalizedName = request.Name.Trim();
        var normalizedDatabase = request.DatabaseName.Trim();
        var featureSettingsJson = string.IsNullOrWhiteSpace(request.FeatureSettingsJson) ? "{}" : request.FeatureSettingsJson;
        var isolationMetadataJson = string.IsNullOrWhiteSpace(request.IsolationMetadataJson) ? "{}" : request.IsolationMetadataJson;

        var existing = await _tenantRegistry.GetByIdentifierAsync(normalizedIdentifier, cancellationToken);

        if (existing is not null && existing.IsActive)
        {
            return new OnboardTenantResult(
                existing.TenantId,
                existing.Identifier,
                existing.Name,
                existing.DatabaseName,
                existing.IsActive,
                true);
        }

        var connectionString = await _tenantConnectionStringBuilder.BuildAsync(normalizedDatabase, cancellationToken);

        TenantRegistryItem registered;
        try
        {
            registered = await _tenantRegistry.UpsertAsync(
                normalizedIdentifier,
                normalizedName,
                normalizedDatabase,
                connectionString,
                featureSettingsJson,
                isolationMetadataJson,
                false,
                cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            var raced = await _tenantRegistry.GetByIdentifierAsync(normalizedIdentifier, cancellationToken);
            if (raced is not null && raced.IsActive)
            {
                return new OnboardTenantResult(
                    raced.TenantId,
                    raced.Identifier,
                    raced.Name,
                    raced.DatabaseName,
                    raced.IsActive,
                    true);
            }

            throw;
        }

        try
        {
            var provisioned = await _tenantOnboardingCoordinator.ExecuteAsync(registered, featureSettingsJson, cancellationToken);
            var activated = await _tenantRegistry.ActivateAsync(provisioned.TenantId, cancellationToken);

            return new OnboardTenantResult(
                activated.TenantId,
                activated.Identifier,
                activated.Name,
                activated.DatabaseName,
                activated.IsActive,
                false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tenant onboarding failed for {TenantIdentifier}", normalizedIdentifier);
            await _tenantRegistry.DeactivateAndDeleteAsync(registered.TenantId, cancellationToken);
            throw;
        }
    }
}
