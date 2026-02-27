namespace cxserver.Application.Abstractions;

public interface ITenantContextAccessor : ITenantContext
{
    void SetTenant(TenantSession tenantSession);
    void Clear();
}
