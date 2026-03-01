using System.Text.Json;
using cxserver.Domain.BlogEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantBlogSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantBlogSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, Guid tenantId, CancellationToken cancellationToken)
    {
        if (!await IsBlogSchemaAvailableAsync(dbContext, cancellationToken))
        {
            return;
        }

        var now = _dateTimeProvider.UtcNow;

        var categories = new (string Name, string Slug)[]
        {
            ("Tech", "tech"),
            ("News", "news"),
            ("Tutorials", "tutorials"),
            ("Reviews", "reviews"),
            ("Tips", "tips")
        };

        var categoryMap = new Dictionary<string, BlogCategory>(StringComparer.OrdinalIgnoreCase);
        foreach (var (name, slug) in categories)
        {
            var category = await dbContext.BlogCategories
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Slug == slug, cancellationToken);

            if (category is null)
            {
                category = BlogCategory.Create(Guid.NewGuid(), tenantId, name, slug, now);
                await dbContext.BlogCategories.AddAsync(category, cancellationToken);
            }
            else
            {
                category.Update(name, slug, true, now);
            }

            categoryMap[slug] = category;
        }

        var tagSeed = new Dictionary<string, string[]>
        {
            ["tech"] = ["hardware", "software", "innovation"],
            ["news"] = ["industry", "launch", "updates"],
            ["tutorials"] = ["how-to", "guide", "setup"],
            ["reviews"] = ["benchmark", "comparison", "hands-on"],
            ["tips"] = ["productivity", "maintenance", "security"]
        };

        var tagMap = new Dictionary<string, BlogTag>(StringComparer.OrdinalIgnoreCase);
        foreach (var (_, tags) in tagSeed)
        {
            foreach (var rawTag in tags)
            {
                var name = ToTitle(rawTag);
                var slug = rawTag.ToLowerInvariant();

                var tag = await dbContext.BlogTags.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Slug == slug, cancellationToken);
                if (tag is null)
                {
                    tag = BlogTag.Create(Guid.NewGuid(), tenantId, name, slug, now);
                    await dbContext.BlogTags.AddAsync(tag, cancellationToken);
                }
                else
                {
                    tag.Update(name, slug, true, now);
                }

                tagMap[slug] = tag;
            }
        }

        var allPosts = new List<(string title, string slug, string category, string[] tags)>();
        var postCounter = 1;
        foreach (var category in categories.Select(x => x.Slug))
        {
            for (var index = 0; index < 4; index++)
            {
                var slug = $"{category}-post-{postCounter}";
                allPosts.Add(($"{ToTitle(category)} Insight {postCounter}", slug, category, tagSeed[category]));
                postCounter++;
            }
        }

        foreach (var postSeed in allPosts.Take(20))
        {
            var post = await dbContext.BlogPosts
                .Include(x => x.PostTags)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Slug == postSeed.slug, cancellationToken);

            var meta = JsonDocument.Parse($$"""["{{postSeed.category}}","codexsun","tenant-blog"]""");
            var body = $"## {postSeed.title}\n\nThis is tenant-scoped blog content for {postSeed.category} with search-ready body text and publish-safe defaults.";
            var excerpt = $"Tenant aware article for {postSeed.category} category.";
            var featured = $"https://images.unsplash.com/photo-1518770660439-4636190af475?w=1600&sig={Math.Abs(postSeed.slug.GetHashCode())}";
            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            if (post is null)
            {
                post = BlogPost.Create(Guid.NewGuid(), tenantId, postSeed.title, postSeed.slug, excerpt, body, featured, categoryMap[postSeed.category].Id, userId, meta, now);
                post.Update(postSeed.title, postSeed.slug, excerpt, body, featured, categoryMap[postSeed.category].Id, meta, true, true, now);
                await dbContext.BlogPosts.AddAsync(post, cancellationToken);
            }
            else
            {
                post.Update(postSeed.title, postSeed.slug, excerpt, body, featured, categoryMap[postSeed.category].Id, meta, true, true, now);
            }

            var desiredTagIds = postSeed.tags.Select(t => tagMap[t].Id).ToHashSet();
            foreach (var existingTag in post.PostTags.ToList())
            {
                if (!desiredTagIds.Contains(existingTag.TagId))
                {
                    existingTag.Delete(now);
                }
            }

            foreach (var tagSlug in postSeed.tags.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var tag = tagMap[tagSlug];
                var postTag = post.PostTags.FirstOrDefault(x => x.TagId == tag.Id);
                if (postTag is null)
                {
                    post.PostTags.Add(BlogPostTag.Create(tenantId, post.Id, tag.Id, now));
                }
                else if (postTag.IsDeleted)
                {
                    postTag.Restore(now);
                }
            }

            if (!post.Images.Any())
            {
                post.Images.Add(BlogPostImage.Create(Guid.NewGuid(), tenantId, post.Id, featured, postSeed.title, "Seeded featured image", 0, now));
                post.Images.Add(BlogPostImage.Create(Guid.NewGuid(), tenantId, post.Id, $"{featured}&v=2", postSeed.title, "Secondary image", 1, now));
            }

            var existingComments = await dbContext.BlogComments
                .Where(x => x.TenantId == tenantId && x.PostId == post.Id)
                .ToListAsync(cancellationToken);

            if (existingComments.Count < 2)
            {
                for (var commentIndex = existingComments.Count; commentIndex < 3; commentIndex++)
                {
                    var comment = BlogComment.Create(
                        Guid.NewGuid(),
                        tenantId,
                        post.Id,
                        Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        $"Seeded comment {commentIndex + 1} for {postSeed.slug}.",
                        now);
                    await dbContext.BlogComments.AddAsync(comment, cancellationToken);
                }
            }

            var existingLike = await dbContext.BlogLikes
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.PostId == post.Id && x.UserId == Guid.Parse("11111111-1111-1111-1111-111111111111"), cancellationToken);

            if (existingLike is null)
            {
                var like = BlogLike.Create(tenantId, post.Id, Guid.Parse("11111111-1111-1111-1111-111111111111"), now);
                like.SetLiked(true, now);
                await dbContext.BlogLikes.AddAsync(like, cancellationToken);
            }
            else
            {
                existingLike.SetLiked(true, now);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<bool> IsBlogSchemaAvailableAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        if (dbContext.Database.IsMySql())
        {
            return await TableExistsMySqlAsync(dbContext, "blog_categories", cancellationToken)
                   && await TableExistsMySqlAsync(dbContext, "blog_posts", cancellationToken);
        }

        if (dbContext.Database.IsNpgsql())
        {
            return await TableExistsPostgresAsync(dbContext, "blog_categories", cancellationToken)
                   && await TableExistsPostgresAsync(dbContext, "blog_posts", cancellationToken);
        }

        return true;
    }

    private static async Task<bool> TableExistsMySqlAsync(TenantDbContext dbContext, string tableName, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT COUNT(*)
                           FROM INFORMATION_SCHEMA.TABLES
                           WHERE TABLE_SCHEMA = DATABASE()
                             AND TABLE_NAME = @tableName
                           """;
        return await ExecuteTableExistsAsync(dbContext, sql, tableName, cancellationToken);
    }

    private static async Task<bool> TableExistsPostgresAsync(TenantDbContext dbContext, string tableName, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT COUNT(*)
                           FROM information_schema.tables
                           WHERE table_schema = 'public'
                             AND table_name = @tableName
                           """;
        return await ExecuteTableExistsAsync(dbContext, sql, tableName, cancellationToken);
    }

    private static async Task<bool> ExecuteTableExistsAsync(TenantDbContext dbContext, string sql, string tableName, CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null && Convert.ToInt32(result) > 0;
    }

    private static string ToTitle(string value)
    {
        return string.Join(" ", value.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => char.ToUpperInvariant(x[0]) + x[1..]));
    }
}
