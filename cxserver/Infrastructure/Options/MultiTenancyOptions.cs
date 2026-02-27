namespace cxserver.Infrastructure.Options;

public sealed class MultiTenancyOptions
{
    public const string SectionName = "MultiTenancy";
    public TenantCacheOptions Cache { get; set; } = new();
    public DefaultTenantOptions DefaultTenant { get; set; } = new();
}

public sealed class DefaultTenantOptions
{
    public string Identifier { get; set; } = "default";
    public string Domain { get; set; } = "default.localhost";
    public string Name { get; set; } = "Default";
    public string DatabaseName { get; set; } = "codexsun_default";
    public string FeatureSettingsJson { get; set; } = "{}";
    public string IsolationMetadataJson { get; set; } = "{}";
}

public sealed class TenantCacheOptions
{
    public int MetadataTtlSeconds { get; set; } = 120;
    public int FeatureTtlSeconds { get; set; } = 60;
}
