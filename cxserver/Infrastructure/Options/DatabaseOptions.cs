namespace cxserver.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";
    public string Provider { get; set; } = "MariaDb";
    public string MariaDbServerVersion { get; set; } = "11.5.2";
    public int MinPoolSize { get; set; } = 5;
    public int MaxPoolSize { get; set; } = 200;
    public MasterDatabaseOptions Master { get; set; } = new();
    public TenantDatabaseOptions Tenant { get; set; } = new();
    public MigrationOptions Migration { get; set; } = new();
}

public sealed class MasterDatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}

public sealed class TenantDatabaseOptions
{
    public string ConnectionStringTemplate { get; set; } = string.Empty;
}

public sealed class MigrationOptions
{
    public int TenantBatchSize { get; set; } = 25;
    public int MaxParallelTenants { get; set; } = 4;
}
