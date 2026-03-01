using System.Text.Json;
using cxserver.Domain.Common;

namespace cxserver.Domain.BlogEngine;

public sealed class BlogPost : AggregateRoot, ISoftDeletable
{
    private BlogPost(
        Guid id,
        Guid tenantId,
        string title,
        string slug,
        string? excerpt,
        string body,
        string? featuredImage,
        Guid categoryId,
        Guid userId,
        JsonDocument? metaKeywords,
        bool published,
        bool active,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        TenantId = tenantId;
        Title = title;
        Slug = slug;
        Excerpt = excerpt;
        Body = body;
        FeaturedImage = featuredImage;
        CategoryId = categoryId;
        UserId = userId;
        MetaKeywords = metaKeywords;
        Published = published;
        Active = active;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private BlogPost() : base(Guid.NewGuid())
    {
        Title = string.Empty;
        Slug = string.Empty;
        Body = string.Empty;
    }

    public Guid TenantId { get; private set; }
    public string Title { get; private set; }
    public string Slug { get; private set; }
    public string? Excerpt { get; private set; }
    public string Body { get; private set; }
    public string? FeaturedImage { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid UserId { get; private set; }
    public JsonDocument? MetaKeywords { get; private set; }
    public bool Published { get; private set; }
    public bool Active { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public BlogCategory Category { get; private set; } = default!;
    public ICollection<BlogPostTag> PostTags { get; } = new List<BlogPostTag>();
    public ICollection<BlogComment> Comments { get; } = new List<BlogComment>();
    public ICollection<BlogLike> Likes { get; } = new List<BlogLike>();
    public ICollection<BlogPostImage> Images { get; } = new List<BlogPostImage>();

    public static BlogPost Create(
        Guid id,
        Guid tenantId,
        string title,
        string slug,
        string? excerpt,
        string body,
        string? featuredImage,
        Guid categoryId,
        Guid userId,
        JsonDocument? metaKeywords,
        DateTimeOffset nowUtc)
    {
        return new BlogPost(
            id,
            tenantId,
            title.Trim(),
            slug.Trim().ToLowerInvariant(),
            string.IsNullOrWhiteSpace(excerpt) ? null : excerpt.Trim(),
            body.Trim(),
            string.IsNullOrWhiteSpace(featuredImage) ? null : featuredImage.Trim(),
            categoryId,
            userId,
            metaKeywords,
            true,
            true,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Update(
        string title,
        string slug,
        string? excerpt,
        string body,
        string? featuredImage,
        Guid categoryId,
        JsonDocument? metaKeywords,
        bool published,
        bool active,
        DateTimeOffset nowUtc)
    {
        Title = title.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Excerpt = string.IsNullOrWhiteSpace(excerpt) ? null : excerpt.Trim();
        Body = body.Trim();
        FeaturedImage = string.IsNullOrWhiteSpace(featuredImage) ? null : featuredImage.Trim();
        CategoryId = categoryId;
        MetaKeywords = metaKeywords;
        Published = published;
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
