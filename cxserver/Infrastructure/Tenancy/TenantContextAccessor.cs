using cxserver.Application.Abstractions;

namespace cxserver.Infrastructure.Tenancy;

internal sealed class TenantContextAccessor : ITenantContextAccessor
{
    private readonly AsyncLocal<TenantSession?> _tenantState = new();

    public TenantSession? Current => _tenantState.Value;
    public Guid? TenantId => Current?.TenantId;
    public string? Identifier => Current?.Identifier;
    public bool HasTenant => Current is not null;

    public void SetTenant(TenantSession tenantSession)
    {
        ArgumentNullException.ThrowIfNull(tenantSession);
        _tenantState.Value = tenantSession;
    }

    public void Clear()
    {
        _tenantState.Value = null;
    }
}
