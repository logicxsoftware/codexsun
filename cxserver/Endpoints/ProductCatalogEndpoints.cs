using cxserver.Application.Abstractions;

namespace cxserver.Endpoints;

public static class ProductCatalogEndpoints
{
    public static IEndpointRouteBuilder MapProductCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (
            HttpContext httpContext,
            [AsParameters] ProductQueryRequestModel query,
            ITenantContext tenantContext,
            IProductCatalogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var attributes = ParseAttributes(httpContext.Request.Query);
            var response = await service.GetProductsAsync(
                tenantContext.TenantId.Value,
                new ProductQueryRequest(
                    query.Category,
                    query.MinPrice,
                    query.MaxPrice,
                    query.Search,
                    query.Sort,
                    query.Page,
                    query.PageSize,
                    attributes),
                cancellationToken);

            return Results.Ok(response);
        });

        app.MapGet("/api/products/{slug}", async (
            string slug,
            ITenantContext tenantContext,
            IProductCatalogQueryService service,
            CancellationToken cancellationToken) =>
        {
            if (tenantContext.TenantId is null)
            {
                return Results.BadRequest(new { message = "Tenant context is required." });
            }

            var response = await service.GetProductBySlugAsync(tenantContext.TenantId.Value, slug, cancellationToken);
            return response is null ? Results.NotFound() : Results.Ok(response);
        });

        return app;
    }

    private static IReadOnlyDictionary<string, string[]> ParseAttributes(IQueryCollection query)
    {
        var result = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in query)
        {
            if (!pair.Key.StartsWith("attr_", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var key = pair.Key["attr_".Length..].Trim().ToLowerInvariant();
            if (key.Length == 0)
            {
                continue;
            }

            var values = pair.Value
                .SelectMany(v => (v ?? string.Empty).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            result[key] = values;
        }

        return result;
    }

    public sealed record ProductQueryRequestModel(
        string? Category,
        decimal? MinPrice,
        decimal? MaxPrice,
        string? Search,
        string? Sort,
        int Page = 1,
        int PageSize = 12);
}
