using System.Text.Json;
using cxserver.Application.Abstractions;

namespace cxserver.Endpoints;

public static class WebNavigationEndpoints
{
    public static RouteGroupBuilder MapWebNavigationEndpoints(this IEndpointRouteBuilder app)
    {
        var admin = app.MapGroup("/api/admin");
        var web = app.MapGroup("/api/web");

        admin.MapGet("/web-navigation-config", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var config = await store.GetWebNavigationConfigAsync(tenantContext.TenantId, cancellationToken);
            return config is null ? Results.NotFound() : Results.Ok(ToResponse(config));
        });

        admin.MapPut("/web-navigation-config", async (UpsertNavigationConfigRequest request, IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            try
            {
                var updated = await store.UpsertWebNavigationConfigAsync(
                    new UpsertNavigationConfigInput(
                        tenantContext.TenantId,
                        request.LayoutConfig,
                        request.StyleConfig,
                        request.BehaviorConfig,
                        request.ComponentConfig,
                        request.IsActive),
                    cancellationToken);

                return Results.Ok(ToResponse(updated));
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(new { message = exception.Message });
            }
        });

        admin.MapPost("/web-navigation-config/default", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var created = await store.ResetWebNavigationConfigAsync(tenantContext.TenantId, cancellationToken);
            return Results.Ok(ToResponse(created));
        });

        admin.MapPost("/web-navigation-config/reset", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var reset = await store.ResetWebNavigationConfigAsync(tenantContext.TenantId, cancellationToken);
            return Results.Ok(ToResponse(reset));
        });

        admin.MapGet("/footer-config", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var config = await store.GetFooterConfigAsync(tenantContext.TenantId, cancellationToken);
            return config is null ? Results.NotFound() : Results.Ok(ToResponse(config));
        });

        admin.MapPut("/footer-config", async (UpsertNavigationConfigRequest request, IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var updated = await store.UpsertFooterConfigAsync(
                new UpsertNavigationConfigInput(
                    tenantContext.TenantId,
                    request.LayoutConfig,
                    request.StyleConfig,
                    request.BehaviorConfig,
                    request.ComponentConfig,
                    request.IsActive),
                cancellationToken);

            return Results.Ok(ToResponse(updated));
        });

        admin.MapPost("/footer-config/default", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var created = await store.ResetFooterConfigAsync(tenantContext.TenantId, cancellationToken);
            return Results.Ok(ToResponse(created));
        });

        admin.MapPost("/footer-config/reset", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var reset = await store.ResetFooterConfigAsync(tenantContext.TenantId, cancellationToken);
            return Results.Ok(ToResponse(reset));
        });

        web.MapGet("/navigation-config", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var config = await store.GetWebNavigationConfigAsync(tenantContext.TenantId, cancellationToken);
            return config is null ? Results.NotFound() : Results.Ok(ToResponse(config));
        });

        web.MapGet("/footer-config", async (IWebsiteNavigationStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var config = await store.GetFooterConfigAsync(tenantContext.TenantId, cancellationToken);
            return config is null ? Results.NotFound() : Results.Ok(ToResponse(config));
        });

        return admin;
    }

    private static NavigationConfigResponse ToResponse(NavigationConfigItem item)
    {
        return new NavigationConfigResponse(
            item.Id,
            item.TenantId,
            ToWidthVariantString(item.WidthVariant),
            item.LayoutConfig,
            item.StyleConfig,
            item.BehaviorConfig,
            item.ComponentConfig,
            item.IsActive,
            item.CreatedAtUtc,
            item.UpdatedAtUtc);
    }

    public sealed record UpsertNavigationConfigRequest(
        JsonDocument LayoutConfig,
        JsonDocument StyleConfig,
        JsonDocument BehaviorConfig,
        JsonDocument ComponentConfig,
        bool IsActive);

    public sealed record NavigationConfigResponse(
        Guid Id,
        Guid? TenantId,
        string WidthVariant,
        JsonDocument LayoutConfig,
        JsonDocument StyleConfig,
        JsonDocument BehaviorConfig,
        JsonDocument ComponentConfig,
        bool IsActive,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset UpdatedAtUtc);

    private static string ToWidthVariantString(Domain.NavigationEngine.NavWidthVariant widthVariant)
    {
        return widthVariant switch
        {
            Domain.NavigationEngine.NavWidthVariant.Full => "full",
            Domain.NavigationEngine.NavWidthVariant.Boxed => "boxed",
            _ => "container",
        };
    }
}
