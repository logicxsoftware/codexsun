namespace cxserver.Application.Abstractions;

public sealed record TenantRegistryItem(
    Guid TenantId,
    string Identifier,
    string Name,
    string ConnectionString,
    bool IsActive,
    string DatabaseName,
    string FeatureSettingsJson,
    string IsolationMetadataJson);
