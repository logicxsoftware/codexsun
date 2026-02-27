namespace cxserver.Application.Abstractions;

public interface ITenantContext
{
    TenantSession? Current { get; }
    Guid? TenantId { get; }
    string? Identifier { get; }
    bool HasTenant { get; }
}
