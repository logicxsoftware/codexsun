using cxserver.Domain.Common;

namespace cxserver.Domain.BlogEngine;

public sealed class BlogCategory : AggregateRoot, ISoftDeletable
{
    private BlogCategory(
        Guid id,
        Guid tenantId,
        string name,
        string slug,
        bool active,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        TenantId = tenantId;
        Name = name;
        Slug = slug;
        Active = active;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private BlogCategory() : base(Guid.NewGuid())
    {
        Name = string.Empty;
        Slug = string.Empty;
    }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public bool Active { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public static BlogCategory Create(Guid id, Guid tenantId, string name, string slug, DateTimeOffset nowUtc)
    {
        return new BlogCategory(
            id,
            tenantId,
            name.Trim(),
            slug.Trim().ToLowerInvariant(),
            true,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Update(string name, string slug, bool active, DateTimeOffset nowUtc)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Active = active;
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

