using System.Text.Json;

namespace cxserver.Application.Abstractions;

public interface IWebsiteNavigationStore
{
    Task<NavigationConfigItem?> GetWebNavigationConfigAsync(Guid? tenantId, CancellationToken cancellationToken);
    Task<NavigationConfigItem> UpsertWebNavigationConfigAsync(UpsertNavigationConfigInput input, CancellationToken cancellationToken);
    Task<NavigationConfigItem> ResetWebNavigationConfigAsync(Guid? tenantId, CancellationToken cancellationToken);

    Task<NavigationConfigItem?> GetFooterConfigAsync(Guid? tenantId, CancellationToken cancellationToken);
    Task<NavigationConfigItem> UpsertFooterConfigAsync(UpsertNavigationConfigInput input, CancellationToken cancellationToken);
    Task<NavigationConfigItem> ResetFooterConfigAsync(Guid? tenantId, CancellationToken cancellationToken);
}

public sealed record NavigationConfigItem(
    Guid Id,
    Guid? TenantId,
    JsonDocument LayoutConfig,
    JsonDocument StyleConfig,
    JsonDocument BehaviorConfig,
    JsonDocument ComponentConfig,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record UpsertNavigationConfigInput(
    Guid? TenantId,
    JsonDocument LayoutConfig,
    JsonDocument StyleConfig,
    JsonDocument BehaviorConfig,
    JsonDocument ComponentConfig,
    bool IsActive);
