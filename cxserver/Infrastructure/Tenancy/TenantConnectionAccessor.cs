namespace cxserver.Infrastructure.Tenancy;

internal sealed class TenantConnectionAccessor : ITenantConnectionAccessor
{
    private readonly AsyncLocal<string?> _connectionState = new();

    public string? ConnectionString => _connectionState.Value;

    public void SetConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        _connectionState.Value = connectionString.Trim();
    }

    public void Clear()
    {
        _connectionState.Value = null;
    }
}
