using System.Text.Json;
using cxserver.Domain.Common;

namespace cxserver.Domain.Configuration;

public sealed class ConfigurationDocument : AggregateRoot, ISoftDeletable
{
    private ConfigurationDocument(
        Guid id,
        string namespaceKey,
        string documentKey,
        JsonDocument payload,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        NamespaceKey = namespaceKey;
        DocumentKey = documentKey;
        Payload = payload;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private ConfigurationDocument() : base(Guid.NewGuid())
    {
        NamespaceKey = string.Empty;
        DocumentKey = string.Empty;
        Payload = JsonDocument.Parse("{}");
    }

    public string NamespaceKey { get; private set; }
    public string DocumentKey { get; private set; }
    public JsonDocument Payload { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public static ConfigurationDocument Create(
        Guid id,
        string namespaceKey,
        string documentKey,
        JsonDocument payload,
        DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(namespaceKey))
        {
            throw new ArgumentException("Namespace key is required.", nameof(namespaceKey));
        }

        if (string.IsNullOrWhiteSpace(documentKey))
        {
            throw new ArgumentException("Document key is required.", nameof(documentKey));
        }

        ArgumentNullException.ThrowIfNull(payload);

        return new ConfigurationDocument(
            id,
            namespaceKey.Trim(),
            documentKey.Trim(),
            payload,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void UpdatePayload(JsonDocument payload, DateTimeOffset updatedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(payload);
        Payload = payload;
        UpdatedAtUtc = updatedAtUtc;
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
