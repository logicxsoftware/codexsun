using cxserver.Application.Abstractions;
using cxserver.Domain.SliderEngine;

namespace cxserver.Endpoints;

public static class SliderEndpoints
{
    public static RouteGroupBuilder MapSliderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/slider");

        group.MapGet("", async (ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var data = await store.GetOrCreateAsync(tenantContext.TenantId, cancellationToken);
            return Results.Ok(data);
        });

        group.MapPut("", async (UpdateSliderConfigRequest request, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var updated = await store.UpdateConfigAsync(
                tenantContext.TenantId,
                new UpdateSliderConfigInput(
                    request.IsActive,
                    request.HeightMode,
                    request.HeightValue,
                    request.ContainerMode,
                    request.ContentAlignment,
                    request.Autoplay,
                    request.Loop,
                    request.ShowProgress,
                    request.ShowNavArrows,
                    request.ShowDots,
                    request.Parallax,
                    request.Particles,
                    request.DefaultVariant,
                    request.DefaultIntensity,
                    request.DefaultDirection,
                    request.DefaultBackgroundMode,
                    request.ScrollBehavior),
                cancellationToken);

            return Results.Ok(updated);
        });

        group.MapPost("/slides", async (CreateSlideRequest request, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var created = await store.CreateSlideAsync(
                tenantContext.TenantId,
                new CreateSlideInput(
                    request.Order,
                    request.Title,
                    request.Tagline,
                    request.ActionText,
                    request.ActionHref,
                    request.CtaColor,
                    request.Duration,
                    request.Direction,
                    request.Variant,
                    request.Intensity,
                    request.BackgroundMode,
                    request.ShowOverlay,
                    request.OverlayToken,
                    request.BackgroundUrl,
                    request.MediaType,
                    request.YoutubeVideoId,
                    request.IsActive,
                    request.Highlights.Select(x => new SlideHighlightInput(x.Text, x.Variant, x.Order)).ToList()),
                cancellationToken);

            return Results.Created($"/api/slider/slides/{created.Id}", created);
        });

        group.MapPatch("/slides/{id:guid}", async (Guid id, UpdateSlideRequest request, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var updated = await store.UpdateSlideAsync(
                tenantContext.TenantId,
                id,
                new UpdateSlideInput(
                    request.Order,
                    request.Title,
                    request.Tagline,
                    request.ActionText,
                    request.ActionHref,
                    request.CtaColor,
                    request.Duration,
                    request.Direction,
                    request.Variant,
                    request.Intensity,
                    request.BackgroundMode,
                    request.ShowOverlay,
                    request.OverlayToken,
                    request.BackgroundUrl,
                    request.MediaType,
                    request.YoutubeVideoId,
                    request.IsActive,
                    request.Highlights.Select(x => new SlideHighlightInput(x.Text, x.Variant, x.Order)).ToList()),
                cancellationToken);

            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapDelete("/slides/{id:guid}", async (Guid id, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var deleted = await store.DeleteSlideAsync(tenantContext.TenantId, id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPatch("/slides/reorder", async (ReorderSlidesRequest request, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            await store.ReorderSlidesAsync(
                tenantContext.TenantId,
                request.Items.Select(x => new ReorderSlideItemInput(x.SlideId, x.Order)).ToList(),
                cancellationToken);
            return Results.NoContent();
        });

        group.MapPost("/layers", async (CreateLayerRequest request, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var created = await store.CreateLayerAsync(
                tenantContext.TenantId,
                new CreateLayerInput(
                    request.SlideId,
                    request.Order,
                    request.Type,
                    request.Content,
                    request.MediaUrl,
                    request.PositionX,
                    request.PositionY,
                    request.Width,
                    request.AnimationFrom,
                    request.AnimationDelay,
                    request.AnimationDuration,
                    request.AnimationEasing,
                    request.ResponsiveVisibility),
                cancellationToken);

            return Results.Created($"/api/slider/layers/{created.Id}", created);
        });

        group.MapPatch("/layers/{id:guid}", async (Guid id, UpdateLayerRequest request, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var updated = await store.UpdateLayerAsync(
                tenantContext.TenantId,
                id,
                new UpdateLayerInput(
                    request.Order,
                    request.Type,
                    request.Content,
                    request.MediaUrl,
                    request.PositionX,
                    request.PositionY,
                    request.Width,
                    request.AnimationFrom,
                    request.AnimationDelay,
                    request.AnimationDuration,
                    request.AnimationEasing,
                    request.ResponsiveVisibility),
                cancellationToken);

            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapDelete("/layers/{id:guid}", async (Guid id, ISliderStore store, ITenantContext tenantContext, CancellationToken cancellationToken) =>
        {
            var deleted = await store.DeleteLayerAsync(tenantContext.TenantId, id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return group;
    }

    public sealed record UpdateSliderConfigRequest(
        bool IsActive,
        SliderHeightMode HeightMode,
        int HeightValue,
        SliderContainerMode ContainerMode,
        SliderContentAlignment ContentAlignment,
        bool Autoplay,
        bool Loop,
        bool ShowProgress,
        bool ShowNavArrows,
        bool ShowDots,
        bool Parallax,
        bool Particles,
        SliderVariant DefaultVariant,
        SliderIntensity DefaultIntensity,
        SliderDirection DefaultDirection,
        SliderBackgroundMode DefaultBackgroundMode,
        SliderScrollBehavior ScrollBehavior);

    public sealed record HighlightRequest(string Text, string Variant, int Order);

    public sealed record CreateSlideRequest(
        int Order,
        string Title,
        string Tagline,
        string? ActionText,
        string? ActionHref,
        SliderCtaColor CtaColor,
        int Duration,
        SliderDirection Direction,
        SliderVariant Variant,
        SliderIntensity Intensity,
        SliderBackgroundMode BackgroundMode,
        bool ShowOverlay,
        string OverlayToken,
        string BackgroundUrl,
        SliderMediaType MediaType,
        string? YoutubeVideoId,
        bool IsActive,
        IReadOnlyList<HighlightRequest> Highlights);

    public sealed record UpdateSlideRequest(
        int Order,
        string Title,
        string Tagline,
        string? ActionText,
        string? ActionHref,
        SliderCtaColor CtaColor,
        int Duration,
        SliderDirection Direction,
        SliderVariant Variant,
        SliderIntensity Intensity,
        SliderBackgroundMode BackgroundMode,
        bool ShowOverlay,
        string OverlayToken,
        string BackgroundUrl,
        SliderMediaType MediaType,
        string? YoutubeVideoId,
        bool IsActive,
        IReadOnlyList<HighlightRequest> Highlights);

    public sealed record ReorderSlideItemRequest(Guid SlideId, int Order);
    public sealed record ReorderSlidesRequest(IReadOnlyList<ReorderSlideItemRequest> Items);

    public sealed record CreateLayerRequest(
        Guid SlideId,
        int Order,
        SliderLayerType Type,
        string Content,
        string? MediaUrl,
        decimal PositionX,
        decimal PositionY,
        string Width,
        SliderLayerAnimationFrom AnimationFrom,
        int AnimationDelay,
        int AnimationDuration,
        string AnimationEasing,
        string ResponsiveVisibility);

    public sealed record UpdateLayerRequest(
        int Order,
        SliderLayerType Type,
        string Content,
        string? MediaUrl,
        decimal PositionX,
        decimal PositionY,
        string Width,
        SliderLayerAnimationFrom AnimationFrom,
        int AnimationDelay,
        int AnimationDuration,
        string AnimationEasing,
        string ResponsiveVisibility);
}
