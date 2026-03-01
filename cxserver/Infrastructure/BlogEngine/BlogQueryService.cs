using System.Data.Common;
using System.Text;
using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.BlogEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.BlogEngine;

internal sealed class BlogQueryService : IBlogQueryService
{
    private readonly ITenantDbContextAccessor _dbContextAccessor;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BlogQueryService(ITenantDbContextAccessor dbContextAccessor, IDateTimeProvider dateTimeProvider)
    {
        _dbContextAccessor = dbContextAccessor;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IReadOnlyList<BlogCategoryItem>> GetCategoriesAsync(Guid tenantId, bool activeOnly, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var query = db.BlogCategories.AsNoTracking().Where(x => x.TenantId == tenantId);

        if (activeOnly)
        {
            query = query.Where(x => x.Active);
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new BlogCategoryItem(x.Id, x.Name, x.Slug, x.Active))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlogTagItem>> GetTagsAsync(Guid tenantId, bool activeOnly, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var query = db.BlogTags.AsNoTracking().Where(x => x.TenantId == tenantId);

        if (activeOnly)
        {
            query = query.Where(x => x.Active);
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new BlogTagItem(x.Id, x.Name, x.Slug, x.Active))
            .ToListAsync(cancellationToken);
    }

    public async Task<BlogPostListResponse> GetPostsAsync(Guid tenantId, BlogPostListRequest request, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var query = db.BlogPosts.AsNoTracking().Where(x => x.TenantId == tenantId && x.Active);

        if (request.PublishedOnly)
        {
            query = query.Where(x => x.Published);
        }

        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
        {
            var normalizedCategory = request.CategorySlug.Trim().ToLowerInvariant();
            query = query.Where(x => x.Category.Slug == normalizedCategory && x.Category.TenantId == tenantId && x.Category.Active);
        }

        if (!string.IsNullOrWhiteSpace(request.TagSlug))
        {
            var normalizedTag = request.TagSlug.Trim().ToLowerInvariant();
            query = query.Where(x => x.PostTags.Any(t => t.Tag.Slug == normalizedTag && t.TenantId == tenantId && t.Tag.Active));
        }

        query = request.Sort.ToLowerInvariant() switch
        {
            "oldest" => query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderByDescending(x => x.CreatedAtUtc),
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BlogPostListItem(
                x.Id,
                x.Title,
                x.Slug,
                x.Excerpt,
                x.FeaturedImage,
                x.Category.Name,
                x.Category.Slug,
                x.CreatedAtUtc,
                x.Likes.Count(l => l.TenantId == tenantId && l.Liked),
                x.Comments.Count(c => c.TenantId == tenantId && c.Approved),
                x.PostTags.Where(t => t.TenantId == tenantId && t.Tag.Active).Select(t => t.Tag.Name).ToList()))
            .ToListAsync(cancellationToken);

        return new BlogPostListResponse(items, BuildPagination(page, pageSize, total));
    }

    public async Task<BlogPostDetailResponse?> GetPostBySlugAsync(Guid tenantId, string slug, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var normalizedSlug = slug.Trim().ToLowerInvariant();

        var post = await db.BlogPosts
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Slug == normalizedSlug && x.Active && x.Published)
            .Include(x => x.Category)
            .Include(x => x.PostTags).ThenInclude(x => x.Tag)
            .Include(x => x.Comments)
            .Include(x => x.Images)
            .Include(x => x.Likes)
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            return null;
        }

        var relatedPosts = await db.BlogPosts
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id != post.Id && x.CategoryId == post.CategoryId && x.Active && x.Published)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(4)
            .Select(x => new BlogPostListItem(
                x.Id,
                x.Title,
                x.Slug,
                x.Excerpt,
                x.FeaturedImage,
                x.Category.Name,
                x.Category.Slug,
                x.CreatedAtUtc,
                x.Likes.Count(l => l.TenantId == tenantId && l.Liked),
                x.Comments.Count(c => c.TenantId == tenantId && c.Approved),
                x.PostTags.Where(t => t.TenantId == tenantId && t.Tag.Active).Select(t => t.Tag.Name).ToList()))
            .ToListAsync(cancellationToken);

        return new BlogPostDetailResponse(
            post.Id,
            post.Title,
            post.Slug,
            post.Excerpt,
            post.Body,
            post.FeaturedImage,
            post.Published,
            post.Active,
            post.CreatedAtUtc,
            post.UpdatedAtUtc,
            post.Category.Name,
            post.Category.Slug,
            post.UserId,
            post.MetaKeywords?.RootElement.Clone(),
            post.PostTags.Where(x => x.TenantId == tenantId && x.Tag.Active).OrderBy(x => x.Tag.Name).Select(x => new BlogTagItem(x.TagId, x.Tag.Name, x.Tag.Slug, x.Tag.Active)).ToList(),
            post.Comments.Where(x => x.TenantId == tenantId && x.Approved).OrderByDescending(x => x.CreatedAtUtc).Select(x => new BlogCommentItem(x.Id, x.PostId, x.UserId, x.Body, x.Approved, x.CreatedAtUtc, x.UpdatedAtUtc)).ToList(),
            post.Images.Where(x => x.TenantId == tenantId).OrderBy(x => x.SortOrder).Select(x => new BlogPostImageItem(x.Id, x.ImagePath, x.AltText, x.Caption, x.SortOrder)).ToList(),
            post.Likes.Count(x => x.TenantId == tenantId && x.Liked),
            relatedPosts);
    }

    public async Task<BlogSearchResponse> SearchPostsAsync(Guid tenantId, BlogSearchRequest request, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        if (db.Database.IsNpgsql())
        {
            return await SearchWithPostgresAsync(db, tenantId, request, page, pageSize, cancellationToken);
        }

        var fallbackQuery = db.BlogPosts.AsNoTracking().Where(x => x.TenantId == tenantId && x.Active);

        if (request.PublishedOnly)
        {
            fallbackQuery = fallbackQuery.Where(x => x.Published);
        }

        var normalized = request.Query.Trim();
        if (normalized.Length > 0)
        {
            fallbackQuery = fallbackQuery.Where(x => x.Title.Contains(normalized) || (x.Excerpt != null && x.Excerpt.Contains(normalized)) || x.Body.Contains(normalized));
        }

        fallbackQuery = request.Sort.ToLowerInvariant() switch
        {
            "oldest" => fallbackQuery.OrderBy(x => x.CreatedAtUtc),
            _ => fallbackQuery.OrderByDescending(x => x.CreatedAtUtc),
        };

        var total = await fallbackQuery.CountAsync(cancellationToken);
        var rows = await fallbackQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BlogSearchItem(x.Id, x.Title, x.Slug, x.Excerpt, x.FeaturedImage, x.Category.Slug, x.CreatedAtUtc, 0m, x.Excerpt))
            .ToListAsync(cancellationToken);

        return new BlogSearchResponse(rows, BuildPagination(page, pageSize, total));
    }

    private async Task<BlogSearchResponse> SearchWithPostgresAsync(TenantDbContext db, Guid tenantId, BlogSearchRequest request, int page, int pageSize, CancellationToken cancellationToken)
    {
        var tsQuery = NormalizeToTsQuery(request.Query);
        var offset = (page - 1) * pageSize;
        var publishedFilter = request.PublishedOnly;
        var category = string.IsNullOrWhiteSpace(request.CategorySlug) ? null : request.CategorySlug.Trim().ToLowerInvariant();
        var tag = string.IsNullOrWhiteSpace(request.TagSlug) ? null : request.TagSlug.Trim().ToLowerInvariant();

        var orderBy = request.Sort.ToLowerInvariant() switch
        {
            "oldest" => "p.created_at_utc ASC",
            "relevance" => "rank DESC, p.created_at_utc DESC",
            _ => "p.created_at_utc DESC",
        };

        var sql = $"""
            SELECT p.id, p.title, p.slug, p.excerpt, p.featured_image, c.slug AS category_slug, p.created_at_utc,
                   ts_rank_cd(p.search_vector, to_tsquery('english', @q)) AS rank,
                   ts_headline('english', coalesce(p.excerpt, p.body), to_tsquery('english', @q)) AS headline
            FROM blog_posts p
            INNER JOIN blog_categories c ON c.id = p.category_id
            WHERE p.tenant_id = @tenant
              AND p.active = TRUE
              AND p.is_deleted = FALSE
              AND c.active = TRUE
              AND c.is_deleted = FALSE
              AND (@publishedOnly = FALSE OR p.published = TRUE)
              AND (@category IS NULL OR c.slug = @category)
              AND (@tag IS NULL OR EXISTS (
                        SELECT 1 FROM blog_post_tags pt
                        INNER JOIN blog_tags t ON t.id = pt.tag_id
                        WHERE pt.post_id = p.id AND pt.tenant_id = @tenant AND t.slug = @tag AND t.active = TRUE AND t.is_deleted = FALSE))
              AND p.search_vector @@ to_tsquery('english', @q)
            ORDER BY {orderBy}
            LIMIT @limit OFFSET @offset;
            """;

        var countSql = """
            SELECT count(1)
            FROM blog_posts p
            INNER JOIN blog_categories c ON c.id = p.category_id
            WHERE p.tenant_id = @tenant
              AND p.active = TRUE
              AND p.is_deleted = FALSE
              AND c.active = TRUE
              AND c.is_deleted = FALSE
              AND (@publishedOnly = FALSE OR p.published = TRUE)
              AND (@category IS NULL OR c.slug = @category)
              AND (@tag IS NULL OR EXISTS (
                        SELECT 1 FROM blog_post_tags pt
                        INNER JOIN blog_tags t ON t.id = pt.tag_id
                        WHERE pt.post_id = p.id AND pt.tenant_id = @tenant AND t.slug = @tag AND t.active = TRUE AND t.is_deleted = FALSE))
              AND p.search_vector @@ to_tsquery('english', @q);
            """;

        var items = new List<BlogSearchItem>();
        var total = 0;

        var connection = db.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using (var countCommand = BuildCommand(connection, countSql, tenantId, tsQuery, publishedFilter, category, tag, pageSize, offset))
        {
            var scalar = await countCommand.ExecuteScalarAsync(cancellationToken);
            total = scalar is null ? 0 : Convert.ToInt32(scalar);
        }

        await using (var command = BuildCommand(connection, sql, tenantId, tsQuery, publishedFilter, category, tag, pageSize, offset))
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(new BlogSearchItem(
                    reader.GetGuid(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetString(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4),
                    reader.GetString(5),
                    reader.GetFieldValue<DateTimeOffset>(6),
                    Convert.ToDecimal(reader.GetFieldValue<float>(7)),
                    reader.IsDBNull(8) ? null : reader.GetString(8)));
            }
        }

        return new BlogSearchResponse(items, BuildPagination(page, pageSize, total));
    }

    public async Task<BlogCategoryItem> UpsertCategoryAsync(Guid tenantId, BlogCategoryUpsertRequest request, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var now = _dateTimeProvider.UtcNow;
        var normalizedName = request.Name.Trim();
        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var duplicate = await db.BlogCategories.AsNoTracking().AnyAsync(x => x.TenantId == tenantId && x.Id != request.Id && (x.Name == normalizedName || x.Slug == normalizedSlug), cancellationToken);
        if (duplicate)
        {
            throw new InvalidOperationException("Category name or slug already exists.");
        }

        BlogCategory entity;
        if (request.Id.HasValue)
        {
            entity = await db.BlogCategories.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == request.Id.Value, cancellationToken)
                     ?? throw new InvalidOperationException("Category not found.");
            entity.Update(normalizedName, normalizedSlug, request.Active, now);
        }
        else
        {
            entity = BlogCategory.Create(Guid.NewGuid(), tenantId, normalizedName, normalizedSlug, now);
            if (!request.Active)
            {
                entity.Update(normalizedName, normalizedSlug, false, now);
            }
            await db.BlogCategories.AddAsync(entity, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        return new BlogCategoryItem(entity.Id, entity.Name, entity.Slug, entity.Active);
    }

    public async Task<BlogTagItem> UpsertTagAsync(Guid tenantId, BlogTagUpsertRequest request, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var now = _dateTimeProvider.UtcNow;
        var normalizedName = request.Name.Trim();
        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var duplicate = await db.BlogTags.AsNoTracking().AnyAsync(x => x.TenantId == tenantId && x.Id != request.Id && (x.Name == normalizedName || x.Slug == normalizedSlug), cancellationToken);
        if (duplicate)
        {
            throw new InvalidOperationException("Tag name or slug already exists.");
        }

        BlogTag entity;
        if (request.Id.HasValue)
        {
            entity = await db.BlogTags.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == request.Id.Value, cancellationToken)
                     ?? throw new InvalidOperationException("Tag not found.");
            entity.Update(normalizedName, normalizedSlug, request.Active, now);
        }
        else
        {
            entity = BlogTag.Create(Guid.NewGuid(), tenantId, normalizedName, normalizedSlug, now);
            if (!request.Active)
            {
                entity.Update(normalizedName, normalizedSlug, false, now);
            }
            await db.BlogTags.AddAsync(entity, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        return new BlogTagItem(entity.Id, entity.Name, entity.Slug, entity.Active);
    }

    public async Task<BlogPostDetailResponse> UpsertPostAsync(Guid tenantId, Guid userId, BlogPostUpsertRequest request, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var now = _dateTimeProvider.UtcNow;
        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var duplicate = await db.BlogPosts.AsNoTracking().AnyAsync(x => x.TenantId == tenantId && x.Id != request.Id && x.Slug == normalizedSlug, cancellationToken);
        if (duplicate)
        {
            throw new InvalidOperationException("Post slug already exists.");
        }

        var categoryExists = await db.BlogCategories.AnyAsync(x => x.TenantId == tenantId && x.Id == request.CategoryId && x.Active, cancellationToken);
        if (!categoryExists)
        {
            throw new InvalidOperationException("Category not found.");
        }

        var tags = await db.BlogTags.Where(x => x.TenantId == tenantId && request.TagIds.Contains(x.Id) && x.Active).ToListAsync(cancellationToken);
        if (tags.Count != request.TagIds.Count)
        {
            throw new InvalidOperationException("One or more tags are invalid.");
        }

        BlogPost entity;
        if (request.Id.HasValue)
        {
            entity = await db.BlogPosts.Include(x => x.PostTags).Include(x => x.Images).FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == request.Id.Value, cancellationToken)
                     ?? throw new InvalidOperationException("Post not found.");

            entity.Update(request.Title, normalizedSlug, request.Excerpt, request.Body, request.FeaturedImage, request.CategoryId, request.MetaKeywords, request.Published, request.Active, now);
        }
        else
        {
            entity = BlogPost.Create(Guid.NewGuid(), tenantId, request.Title, normalizedSlug, request.Excerpt, request.Body, request.FeaturedImage, request.CategoryId, userId, request.MetaKeywords, now);
            entity.Update(request.Title, normalizedSlug, request.Excerpt, request.Body, request.FeaturedImage, request.CategoryId, request.MetaKeywords, request.Published, request.Active, now);
            await db.BlogPosts.AddAsync(entity, cancellationToken);
        }

        var existingPostTags = entity.PostTags.Where(x => x.TenantId == tenantId).ToList();
        foreach (var postTag in existingPostTags)
        {
            if (!request.TagIds.Contains(postTag.TagId))
            {
                postTag.Delete(now);
            }
        }

        foreach (var tagId in request.TagIds)
        {
            var existing = existingPostTags.FirstOrDefault(x => x.TagId == tagId);
            if (existing is null)
            {
                entity.PostTags.Add(BlogPostTag.Create(tenantId, entity.Id, tagId, now));
            }
            else if (existing.IsDeleted)
            {
                existing.Restore(now);
            }
        }

        var existingImages = entity.Images.Where(x => x.TenantId == tenantId).ToDictionary(x => x.Id);
        var requestedIds = request.Images.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

        foreach (var existing in existingImages.Values.ToList())
        {
            if (!requestedIds.Contains(existing.Id))
            {
                db.BlogPostImages.Remove(existing);
            }
        }

        foreach (var image in request.Images)
        {
            if (image.Id.HasValue && existingImages.TryGetValue(image.Id.Value, out var tracked))
            {
                db.Entry(tracked).CurrentValues.SetValues(new
                {
                    tracked.Id,
                    tracked.TenantId,
                    tracked.PostId,
                    ImagePath = image.ImagePath.Trim(),
                    AltText = string.IsNullOrWhiteSpace(image.AltText) ? null : image.AltText.Trim(),
                    Caption = string.IsNullOrWhiteSpace(image.Caption) ? null : image.Caption.Trim(),
                    SortOrder = image.SortOrder,
                    tracked.CreatedAtUtc,
                    UpdatedAtUtc = now
                });
            }
            else
            {
                entity.Images.Add(BlogPostImage.Create(Guid.NewGuid(), tenantId, entity.Id, image.ImagePath, image.AltText, image.Caption, image.SortOrder, now));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return await GetPostBySlugAsync(tenantId, entity.Slug, cancellationToken)
               ?? throw new InvalidOperationException("Post was saved but could not be reloaded.");
    }

    public async Task<bool> DeleteCategoryAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var entity = await db.BlogCategories.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.Delete(_dateTimeProvider.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteTagAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var entity = await db.BlogTags.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.Delete(_dateTimeProvider.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeletePostAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var entity = await db.BlogPosts.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.Delete(_dateTimeProvider.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<BlogCommentItem> UpsertCommentAsync(Guid tenantId, Guid userId, BlogCommentUpsertRequest request, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var now = _dateTimeProvider.UtcNow;

        BlogComment comment;
        if (request.Id.HasValue)
        {
            comment = await db.BlogComments.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == request.Id.Value, cancellationToken)
                      ?? throw new InvalidOperationException("Comment not found.");
            comment.Update(request.Body, request.Approved, now);
        }
        else
        {
            var postExists = await db.BlogPosts.AnyAsync(x => x.TenantId == tenantId && x.Id == request.PostId && x.Active, cancellationToken);
            if (!postExists)
            {
                throw new InvalidOperationException("Post not found.");
            }

            comment = BlogComment.Create(Guid.NewGuid(), tenantId, request.PostId, userId, request.Body, now);
            if (!request.Approved)
            {
                comment.Update(request.Body, false, now);
            }
            await db.BlogComments.AddAsync(comment, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        return new BlogCommentItem(comment.Id, comment.PostId, comment.UserId, comment.Body, comment.Approved, comment.CreatedAtUtc, comment.UpdatedAtUtc);
    }

    public async Task<bool> DeleteCommentAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var comment = await db.BlogComments.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, cancellationToken);
        if (comment is null) return false;
        comment.Delete(_dateTimeProvider.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<BlogLikeItem> SetLikeAsync(Guid tenantId, Guid userId, BlogLikeUpsertRequest request, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var now = _dateTimeProvider.UtcNow;
        var postExists = await db.BlogPosts.AnyAsync(x => x.TenantId == tenantId && x.Id == request.PostId && x.Active, cancellationToken);
        if (!postExists)
        {
            throw new InvalidOperationException("Post not found.");
        }

        var like = await db.BlogLikes.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.PostId == request.PostId && x.UserId == userId, cancellationToken);
        if (like is null)
        {
            like = BlogLike.Create(tenantId, request.PostId, userId, now);
            like.SetLiked(request.Liked, now);
            await db.BlogLikes.AddAsync(like, cancellationToken);
        }
        else
        {
            if (like.IsDeleted)
            {
                like.Restore(now);
            }
            like.SetLiked(request.Liked, now);
        }

        await db.SaveChangesAsync(cancellationToken);
        return new BlogLikeItem(like.PostId, like.UserId, like.Liked, like.UpdatedAtUtc);
    }

    public async Task<bool> DeleteLikeAsync(Guid tenantId, Guid postId, Guid userId, CancellationToken cancellationToken)
    {
        var db = await _dbContextAccessor.GetAsync(cancellationToken);
        var like = await db.BlogLikes.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.PostId == postId && x.UserId == userId, cancellationToken);
        if (like is null) return false;
        like.Delete(_dateTimeProvider.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static DbCommand BuildCommand(DbConnection connection, string sql, Guid tenantId, string tsQuery, bool publishedOnly, string? category, string? tag, int limit, int offset)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;

        var tenant = command.CreateParameter(); tenant.ParameterName = "@tenant"; tenant.Value = tenantId; command.Parameters.Add(tenant);
        var q = command.CreateParameter(); q.ParameterName = "@q"; q.Value = tsQuery; command.Parameters.Add(q);
        var published = command.CreateParameter(); published.ParameterName = "@publishedOnly"; published.Value = publishedOnly; command.Parameters.Add(published);
        var categoryParam = command.CreateParameter(); categoryParam.ParameterName = "@category"; categoryParam.Value = category ?? (object)DBNull.Value; command.Parameters.Add(categoryParam);
        var tagParam = command.CreateParameter(); tagParam.ParameterName = "@tag"; tagParam.Value = tag ?? (object)DBNull.Value; command.Parameters.Add(tagParam);
        var limitParam = command.CreateParameter(); limitParam.ParameterName = "@limit"; limitParam.Value = limit; command.Parameters.Add(limitParam);
        var offsetParam = command.CreateParameter(); offsetParam.ParameterName = "@offset"; offsetParam.Value = offset; command.Parameters.Add(offsetParam);
        return command;
    }

    private static string NormalizeToTsQuery(string query)
    {
        var value = query.Trim();
        if (value.Length == 0) return "''";

        var tokens = Tokenize(value);
        var builder = new StringBuilder();
        for (var index = 0; index < tokens.Count; index++)
        {
            var token = tokens[index];
            if (token is "|" or "&" or "!" or "(" or ")")
            {
                builder.Append(token);
            }
            else
            {
                var escaped = token.Replace("'", "''", StringComparison.Ordinal);
                if (escaped.EndsWith('*')) escaped = $"{escaped[..^1]}:*";
                builder.Append(escaped);
            }

            if (index < tokens.Count - 1) builder.Append(' ');
        }

        return builder.ToString();
    }

    private static List<string> Tokenize(string query)
    {
        var tokens = new List<string>();
        var buffer = new StringBuilder();
        var inPhrase = false;

        for (var i = 0; i < query.Length; i++)
        {
            var ch = query[i];
            if (ch == '"')
            {
                if (inPhrase && buffer.Length > 0)
                {
                    tokens.Add($"'{buffer.ToString().Trim().Replace(" ", " <-> ", StringComparison.Ordinal)}'");
                    buffer.Clear();
                }

                inPhrase = !inPhrase;
                continue;
            }

            if (!inPhrase && char.IsWhiteSpace(ch))
            {
                FlushBufferedToken(buffer, tokens);
                continue;
            }

            if (!inPhrase && ch == '-')
            {
                FlushBufferedToken(buffer, tokens);
                tokens.Add("!");
                continue;
            }

            if (!inPhrase && ch == '|')
            {
                FlushBufferedToken(buffer, tokens);
                tokens.Add("|");
                continue;
            }

            if (!inPhrase && ch == '&')
            {
                FlushBufferedToken(buffer, tokens);
                tokens.Add("&");
                continue;
            }

            buffer.Append(ch);
        }

        FlushBufferedToken(buffer, tokens);

        var withImplicitAnd = new List<string>();
        for (var i = 0; i < tokens.Count; i++)
        {
            withImplicitAnd.Add(tokens[i]);
            if (i < tokens.Count - 1)
            {
                var current = tokens[i];
                var next = tokens[i + 1];
                if (current != "|" && current != "&" && current != "!" && next != "|" && next != "&")
                {
                    withImplicitAnd.Add("&");
                }
            }
        }

        return withImplicitAnd;
    }

    private static void FlushBufferedToken(StringBuilder buffer, ICollection<string> tokens)
    {
        if (buffer.Length == 0) return;
        var token = buffer.ToString().Trim();
        buffer.Clear();
        if (token.Length == 0) return;
        tokens.Add(token);
    }

    private static BlogPagination BuildPagination(int page, int pageSize, int total)
    {
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        return new BlogPagination(page, pageSize, total, totalPages, page > 1, page < totalPages);
    }
}
