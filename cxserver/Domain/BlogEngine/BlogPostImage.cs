namespace cxserver.Domain.BlogEngine;

public sealed class BlogPostImage
{
    private BlogPostImage(
        Guid id,
        Guid tenantId,
        Guid postId,
        string imagePath,
        string? altText,
        string? caption,
        int sortOrder,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        Id = id;
        TenantId = tenantId;
        PostId = postId;
        ImagePath = imagePath;
        AltText = altText;
        Caption = caption;
        SortOrder = sortOrder;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    private BlogPostImage()
    {
        ImagePath = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid PostId { get; private set; }
    public string ImagePath { get; private set; }
    public string? AltText { get; private set; }
    public string? Caption { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public BlogPost Post { get; private set; } = default!;

    public static BlogPostImage Create(
        Guid id,
        Guid tenantId,
        Guid postId,
        string imagePath,
        string? altText,
        string? caption,
        int sortOrder,
        DateTimeOffset nowUtc)
    {
        return new BlogPostImage(
            id,
            tenantId,
            postId,
            imagePath.Trim(),
            string.IsNullOrWhiteSpace(altText) ? null : altText.Trim(),
            string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
            sortOrder,
            nowUtc,
            nowUtc);
    }
}

