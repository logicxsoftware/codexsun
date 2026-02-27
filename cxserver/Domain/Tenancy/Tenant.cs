using System.Text.Json;
using cxserver.Domain.Common;

namespace cxserver.Domain.Tenancy;

public sealed class Tenant : AggregateRoot, ISoftDeletable
{
    private Tenant(
        Guid id,
        string identifier,
        string name,
        string databaseName,
        string connectionString,
        TenantStatus status,
        JsonDocument featureSettings,
        JsonDocument isolationMetadata,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        Identifier = identifier;
        Name = name;
        DatabaseName = databaseName;
        ConnectionString = connectionString;
        Status = status;
        FeatureSettings = featureSettings;
        IsolationMetadata = isolationMetadata;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private Tenant() : base(Guid.NewGuid())
    {
        Identifier = string.Empty;
        Name = string.Empty;
        DatabaseName = string.Empty;
        ConnectionString = string.Empty;
        FeatureSettings = JsonDocument.Parse("{}");
        IsolationMetadata = JsonDocument.Parse("{}");
    }

    public string Identifier { get; private set; }
    public string Name { get; private set; }
    public string DatabaseName { get; private set; }
    public string ConnectionString { get; private set; }
    public TenantStatus Status { get; private set; }
    public JsonDocument FeatureSettings { get; private set; }
    public JsonDocument IsolationMetadata { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public static Tenant Create(
        Guid id,
        string identifier,
        string name,
        string databaseName,
        string connectionString,
        JsonDocument featureSettings,
        JsonDocument isolationMetadata,
        DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentException("Identifier is required.", nameof(identifier));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("Database name is required.", nameof(databaseName));
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required.", nameof(connectionString));
        ArgumentNullException.ThrowIfNull(featureSettings);
        ArgumentNullException.ThrowIfNull(isolationMetadata);

        return new Tenant(
            id,
            identifier.Trim(),
            name.Trim(),
            databaseName.Trim(),
            connectionString.Trim(),
            TenantStatus.Active,
            featureSettings,
            isolationMetadata,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void UpdateDisplayName(string name, DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        Name = name.Trim();
        UpdatedAtUtc = nowUtc;
    }

    public void UpdateConnection(string databaseName, string connectionString, DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("Database name is required.", nameof(databaseName));
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required.", nameof(connectionString));

        DatabaseName = databaseName.Trim();
        ConnectionString = connectionString.Trim();
        UpdatedAtUtc = nowUtc;
    }

    public void UpdateFeatureSettings(JsonDocument featureSettings, DateTimeOffset nowUtc)
    {
        ArgumentNullException.ThrowIfNull(featureSettings);
        FeatureSettings = featureSettings;
        UpdatedAtUtc = nowUtc;
    }

    public void UpdateIsolationMetadata(JsonDocument isolationMetadata, DateTimeOffset nowUtc)
    {
        ArgumentNullException.ThrowIfNull(isolationMetadata);
        IsolationMetadata = isolationMetadata;
        UpdatedAtUtc = nowUtc;
    }

    public void Activate(DateTimeOffset nowUtc)
    {
        Status = TenantStatus.Active;
        UpdatedAtUtc = nowUtc;
    }

    public void Deactivate(DateTimeOffset nowUtc)
    {
        Status = TenantStatus.Inactive;
        UpdatedAtUtc = nowUtc;
    }

    public void Delete(DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
        UpdatedAtUtc = deletedAtUtc;
    }

    public void Restore(DateTimeOffset restoredAtUtc)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = restoredAtUtc;
    }
}
