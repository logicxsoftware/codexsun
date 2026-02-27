namespace cxserver.Application.Abstractions;

public sealed record OnboardTenantResult(
    Guid TenantId,
    string Identifier,
    string Name,
    string DatabaseName,
    bool IsActive,
    bool IsExisting);
