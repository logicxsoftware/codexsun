using cxserver.Domain.Common;

namespace cxserver.Domain.BlogEngine;

public sealed class BlogComment : AggregateRoot, ISoftDeletable
{
    private BlogComment(
        Guid id,
        Guid tenantId,
        Guid postId,
        Guid userId,
        string body,
        bool approved,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        TenantId = tenantId;
        PostId = postId;
        UserId = userId;
        Body = body;
        Approved = approved;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private BlogComment() : base(Guid.NewGuid())
    {
        Body = string.Empty;
    }

    public Guid TenantId { get; private set; }
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public string Body { get; private set; }
    public bool Approved { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public BlogPost Post { get; private set; } = default!;

    public static BlogComment Create(Guid id, Guid tenantId, Guid postId, Guid userId, string body, DateTimeOffset nowUtc)
    {
        return new BlogComment(id, tenantId, postId, userId, body.Trim(), true, nowUtc, nowUtc, false, null);
    }

    public void Update(string body, bool approved, DateTimeOffset nowUtc)
    {
        Body = body.Trim();
        Approved = approved;
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

