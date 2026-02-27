using cxserver.Application.Abstractions;

namespace cxserver.Infrastructure.Tenancy;

internal sealed class TenantContextAccessor : ITenantContextAccessor
{
    private readonly object _sync = new();
    private TenantSession? _current;

    public TenantSession? Current => _current;
    public Guid? TenantId => Current?.TenantId;
    public string? TenantName => Current?.TenantName;
    public string? Domain => Current?.Domain;
    public string? TenantDatabaseConnectionString => Current?.TenantDatabaseConnectionString;
    public bool HasTenant => Current is not null;

    public void SetTenant(TenantSession tenantSession)
    {
        ArgumentNullException.ThrowIfNull(tenantSession);

        lock (_sync)
        {
            if (_current is not null)
            {
                throw new InvalidOperationException("Tenant context is already set for this request.");
            }

            _current = tenantSession;
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _current = null;
        }
    }
}
