using System.Text.Json;
using cxserver.Application.Abstractions;

namespace cxserver.Endpoints;

public static class BlogEndpoints
{
    public static IEndpointRouteBuilder MapBlogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/blog");

        group.MapGet("/categories", async (
            ITenantContext tenantContext,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.GetCategoriesAsync(tenantContext.TenantId.Value, true, cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/tags", async (
            ITenantContext tenantContext,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.GetTagsAsync(tenantContext.TenantId.Value, true, cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/posts", async (
            [AsParameters] BlogListRequest request,
            ITenantContext tenantContext,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.GetPostsAsync(
                tenantContext.TenantId.Value,
                new BlogPostListRequest(request.Category, request.Tag, request.Sort ?? "newest", request.Page, request.PageSize, true),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapGet("/posts/{slug}", async (
            string slug,
            ITenantContext tenantContext,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.GetPostBySlugAsync(tenantContext.TenantId.Value, slug, cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/search", async (
            [AsParameters] BlogSearchApiRequest request,
            ITenantContext tenantContext,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var pageSize = Math.Clamp(request.PageSize, 1, 50);
            var result = await service.SearchPostsAsync(
                tenantContext.TenantId.Value,
                new BlogSearchRequest(request.Q ?? string.Empty, request.Category, request.Tag, request.Sort ?? "relevance", request.Page, pageSize, true),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapPost("/categories", async (
            BlogCategoryWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.UpsertCategoryAsync(
                tenantContext.TenantId.Value,
                new BlogCategoryUpsertRequest(request.Id, request.Name, request.Slug, request.Active),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapPut("/categories/{id:guid}", async (
            Guid id,
            BlogCategoryWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.UpsertCategoryAsync(
                tenantContext.TenantId.Value,
                new BlogCategoryUpsertRequest(id, request.Name, request.Slug, request.Active),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/categories/{id:guid}", async (
            Guid id,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var deleted = await service.DeleteCategoryAsync(tenantContext.TenantId.Value, id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/tags", async (
            BlogTagWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.UpsertTagAsync(
                tenantContext.TenantId.Value,
                new BlogTagUpsertRequest(request.Id, request.Name, request.Slug, request.Active),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/tags/{id:guid}", async (
            Guid id,
            BlogTagWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var result = await service.UpsertTagAsync(
                tenantContext.TenantId.Value,
                new BlogTagUpsertRequest(id, request.Name, request.Slug, request.Active),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/tags/{id:guid}", async (
            Guid id,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var deleted = await service.DeleteTagAsync(tenantContext.TenantId.Value, id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/posts", async (
            BlogPostWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }
            if (!Guid.TryParse(currentUser.UserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await service.UpsertPostAsync(
                tenantContext.TenantId.Value,
                userId,
                ToPostUpsert(request, null),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/posts/{id:guid}", async (
            Guid id,
            BlogPostWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }
            if (!Guid.TryParse(currentUser.UserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await service.UpsertPostAsync(
                tenantContext.TenantId.Value,
                userId,
                ToPostUpsert(request, id),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/posts/{id:guid}", async (
            Guid id,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var deleted = await service.DeletePostAsync(tenantContext.TenantId.Value, id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/comments", async (
            BlogCommentWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }
            if (!Guid.TryParse(currentUser.UserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await service.UpsertCommentAsync(
                tenantContext.TenantId.Value,
                userId,
                new BlogCommentUpsertRequest(request.Id, request.PostId, request.Body, request.Approved),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapPut("/comments/{id:guid}", async (
            Guid id,
            BlogCommentWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }
            if (!Guid.TryParse(currentUser.UserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await service.UpsertCommentAsync(
                tenantContext.TenantId.Value,
                userId,
                new BlogCommentUpsertRequest(id, request.PostId, request.Body, request.Approved),
                cancellationToken);
            return Results.Ok(result);
        });

        group.MapDelete("/comments/{id:guid}", async (
            Guid id,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var deleted = await service.DeleteCommentAsync(tenantContext.TenantId.Value, id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/likes", async (
            BlogLikeWriteRequest request,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }
            if (!Guid.TryParse(currentUser.UserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await service.SetLikeAsync(
                tenantContext.TenantId.Value,
                userId,
                new BlogLikeUpsertRequest(request.PostId, request.Liked),
                cancellationToken);

            return Results.Ok(result);
        });

        group.MapDelete("/likes/{postId:guid}", async (
            Guid postId,
            ITenantContext tenantContext,
            ICurrentUser currentUser,
            IBlogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (!currentUser.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }
            if (!Guid.TryParse(currentUser.UserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var deleted = await service.DeleteLikeAsync(tenantContext.TenantId.Value, postId, userId, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }

    private static BlogPostUpsertRequest ToPostUpsert(BlogPostWriteRequest request, Guid? id)
    {
        JsonDocument? meta = null;
        if (!string.IsNullOrWhiteSpace(request.MetaKeywordsJson))
        {
            meta = JsonDocument.Parse(request.MetaKeywordsJson);
        }

        return new BlogPostUpsertRequest(
            id,
            request.Title,
            request.Slug,
            request.Excerpt,
            request.Body,
            request.FeaturedImage,
            request.CategoryId,
            meta,
            request.Published,
            request.Active,
            request.TagIds,
            request.Images.Select(x => new BlogPostUpsertImageRequest(x.Id, x.ImagePath, x.AltText, x.Caption, x.SortOrder)).ToList());
    }

    public sealed record BlogListRequest(string? Category, string? Tag, string? Sort, int Page = 1, int PageSize = 12);

    public sealed record BlogSearchApiRequest(
        string? Q,
        int Page = 1,
        int PageSize = 12,
        string? Category = null,
        string? Tag = null,
        string? Sort = "relevance");

    public sealed record BlogCategoryWriteRequest(Guid? Id, string Name, string Slug, bool Active = true);
    public sealed record BlogTagWriteRequest(Guid? Id, string Name, string Slug, bool Active = true);

    public sealed record BlogPostImageWriteRequest(Guid? Id, string ImagePath, string? AltText, string? Caption, int SortOrder = 0);

    public sealed record BlogPostWriteRequest(
        string Title,
        string Slug,
        string? Excerpt,
        string Body,
        string? FeaturedImage,
        Guid CategoryId,
        string? MetaKeywordsJson,
        bool Published,
        bool Active,
        IReadOnlyList<Guid> TagIds,
        IReadOnlyList<BlogPostImageWriteRequest> Images);

    public sealed record BlogCommentWriteRequest(Guid? Id, Guid PostId, string Body, bool Approved = true);
    public sealed record BlogLikeWriteRequest(Guid PostId, bool Liked = true);
}
