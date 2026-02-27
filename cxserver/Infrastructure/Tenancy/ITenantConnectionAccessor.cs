namespace cxserver.Infrastructure.Tenancy;

public interface ITenantConnectionAccessor
{
    string? ConnectionString { get; }
    void SetConnectionString(string connectionString);
    void Clear();
}
