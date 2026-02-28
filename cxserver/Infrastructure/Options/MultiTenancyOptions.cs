namespace cxserver.Infrastructure.Options;

public sealed class MultiTenancyOptions
{
    public const string SectionName = "MultiTenancy";
    public TenantCacheOptions Cache { get; set; } = new();
    public DefaultTenantOptions DefaultTenant { get; set; } = new();
}

public sealed class DefaultTenantOptions
{
    public string Identifier { get; set; } = "codexsun";
    public string Domain { get; set; } = "localhost";
    public string Name { get; set; } = "Codexsun";
    public string DatabaseName { get; set; } = "tenant1_db";
    public string FeatureSettingsJson { get; set; } = "{}";
    public string IsolationMetadataJson { get; set; } = "{}";
}

public sealed class TenantCacheOptions
{
    public int MetadataTtlSeconds { get; set; } = 120;
    public int FeatureTtlSeconds { get; set; } = 60;
}
