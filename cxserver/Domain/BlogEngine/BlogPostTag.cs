using cxserver.Domain.Common;

namespace cxserver.Domain.BlogEngine;

public sealed class BlogPostTag : ISoftDeletable
{
    private BlogPostTag(
        Guid tenantId,
        Guid postId,
        Guid tagId,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        TenantId = tenantId;
        PostId = postId;
        TagId = tagId;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private BlogPostTag()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid PostId { get; private set; }
    public Guid TagId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public BlogPost Post { get; private set; } = default!;
    public BlogTag Tag { get; private set; } = default!;

    public static BlogPostTag Create(Guid tenantId, Guid postId, Guid tagId, DateTimeOffset nowUtc)
    {
        return new BlogPostTag(tenantId, postId, tagId, nowUtc, nowUtc, false, null);
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

