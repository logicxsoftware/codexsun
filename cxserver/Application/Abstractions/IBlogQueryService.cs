using System.Text.Json;

namespace cxserver.Application.Abstractions;

public interface IBlogQueryService
{
    Task<IReadOnlyList<BlogCategoryItem>> GetCategoriesAsync(Guid tenantId, bool activeOnly, CancellationToken cancellationToken);
    Task<IReadOnlyList<BlogTagItem>> GetTagsAsync(Guid tenantId, bool activeOnly, CancellationToken cancellationToken);
    Task<BlogPostListResponse> GetPostsAsync(Guid tenantId, BlogPostListRequest request, CancellationToken cancellationToken);
    Task<BlogPostDetailResponse?> GetPostBySlugAsync(Guid tenantId, string slug, CancellationToken cancellationToken);
    Task<BlogSearchResponse> SearchPostsAsync(Guid tenantId, BlogSearchRequest request, CancellationToken cancellationToken);
    Task<BlogCategoryItem> UpsertCategoryAsync(Guid tenantId, BlogCategoryUpsertRequest request, CancellationToken cancellationToken);
    Task<BlogTagItem> UpsertTagAsync(Guid tenantId, BlogTagUpsertRequest request, CancellationToken cancellationToken);
    Task<BlogPostDetailResponse> UpsertPostAsync(Guid tenantId, Guid userId, BlogPostUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteCategoryAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task<bool> DeleteTagAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task<bool> DeletePostAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task<BlogCommentItem> UpsertCommentAsync(Guid tenantId, Guid userId, BlogCommentUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteCommentAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
    Task<BlogLikeItem> SetLikeAsync(Guid tenantId, Guid userId, BlogLikeUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteLikeAsync(Guid tenantId, Guid postId, Guid userId, CancellationToken cancellationToken);
}

public sealed record BlogPostListRequest(
    string? CategorySlug,
    string? TagSlug,
    string Sort,
    int Page,
    int PageSize,
    bool PublishedOnly);

public sealed record BlogSearchRequest(
    string Query,
    string? CategorySlug,
    string? TagSlug,
    string Sort,
    int Page,
    int PageSize,
    bool PublishedOnly);

public sealed record BlogPostListResponse(
    IReadOnlyList<BlogPostListItem> Data,
    BlogPagination Pagination);

public sealed record BlogSearchResponse(
    IReadOnlyList<BlogSearchItem> Data,
    BlogPagination Pagination);

public sealed record BlogPagination(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPrevious,
    bool HasNext);

public sealed record BlogPostListItem(
    Guid Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? FeaturedImage,
    string CategoryName,
    string CategorySlug,
    DateTimeOffset CreatedAtUtc,
    int LikeCount,
    int CommentCount,
    IReadOnlyList<string> Tags);

public sealed record BlogSearchItem(
    Guid Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? FeaturedImage,
    string CategorySlug,
    DateTimeOffset CreatedAtUtc,
    decimal Rank,
    string? Headline);

public sealed record BlogPostDetailResponse(
    Guid Id,
    string Title,
    string Slug,
    string? Excerpt,
    string Body,
    string? FeaturedImage,
    bool Published,
    bool Active,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    string CategoryName,
    string CategorySlug,
    Guid UserId,
    JsonElement? MetaKeywords,
    IReadOnlyList<BlogTagItem> Tags,
    IReadOnlyList<BlogCommentItem> Comments,
    IReadOnlyList<BlogPostImageItem> Images,
    int LikeCount,
    IReadOnlyList<BlogPostListItem> RelatedPosts);

public sealed record BlogCategoryItem(Guid Id, string Name, string Slug, bool Active);
public sealed record BlogTagItem(Guid Id, string Name, string Slug, bool Active);

public sealed record BlogCommentItem(
    Guid Id,
    Guid PostId,
    Guid UserId,
    string Body,
    bool Approved,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record BlogLikeItem(
    Guid PostId,
    Guid UserId,
    bool Liked,
    DateTimeOffset UpdatedAtUtc);

public sealed record BlogPostImageItem(
    Guid Id,
    string ImagePath,
    string? AltText,
    string? Caption,
    int SortOrder);

public sealed record BlogCategoryUpsertRequest(
    Guid? Id,
    string Name,
    string Slug,
    bool Active);

public sealed record BlogTagUpsertRequest(
    Guid? Id,
    string Name,
    string Slug,
    bool Active);

public sealed record BlogPostUpsertImageRequest(
    Guid? Id,
    string ImagePath,
    string? AltText,
    string? Caption,
    int SortOrder);

public sealed record BlogPostUpsertRequest(
    Guid? Id,
    string Title,
    string Slug,
    string? Excerpt,
    string Body,
    string? FeaturedImage,
    Guid CategoryId,
    JsonDocument? MetaKeywords,
    bool Published,
    bool Active,
    IReadOnlyList<Guid> TagIds,
    IReadOnlyList<BlogPostUpsertImageRequest> Images);

public sealed record BlogCommentUpsertRequest(
    Guid? Id,
    Guid PostId,
    string Body,
    bool Approved);

public sealed record BlogLikeUpsertRequest(
    Guid PostId,
    bool Liked);
