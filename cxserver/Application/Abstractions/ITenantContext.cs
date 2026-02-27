namespace cxserver.Application.Abstractions;

public interface ITenantContext
{
    TenantSession? Current { get; }
    Guid? TenantId { get; }
    string? TenantName { get; }
    string? Domain { get; }
    string? TenantDatabaseConnectionString { get; }
    bool HasTenant { get; }
}
