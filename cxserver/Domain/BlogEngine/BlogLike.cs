using cxserver.Domain.Common;

namespace cxserver.Domain.BlogEngine;

public sealed class BlogLike : ISoftDeletable
{
    private BlogLike(
        Guid tenantId,
        Guid postId,
        Guid userId,
        bool liked,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        TenantId = tenantId;
        PostId = postId;
        UserId = userId;
        Liked = liked;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private BlogLike()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public bool Liked { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public BlogPost Post { get; private set; } = default!;

    public static BlogLike Create(Guid tenantId, Guid postId, Guid userId, DateTimeOffset nowUtc)
    {
        return new BlogLike(tenantId, postId, userId, true, nowUtc, nowUtc, false, null);
    }

    public void SetLiked(bool liked, DateTimeOffset nowUtc)
    {
        Liked = liked;
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

