using cxserver.Domain.SliderEngine;

namespace cxserver.Application.Abstractions;

public interface ISliderStore
{
    Task<SliderConfigDto> GetOrCreateAsync(Guid? tenantId, CancellationToken cancellationToken);
    Task<SliderConfigDto> UpdateConfigAsync(Guid? tenantId, UpdateSliderConfigInput input, CancellationToken cancellationToken);
    Task<SlideDto> CreateSlideAsync(Guid? tenantId, CreateSlideInput input, CancellationToken cancellationToken);
    Task<SlideDto?> UpdateSlideAsync(Guid? tenantId, Guid slideId, UpdateSlideInput input, CancellationToken cancellationToken);
    Task<bool> DeleteSlideAsync(Guid? tenantId, Guid slideId, CancellationToken cancellationToken);
    Task<bool> ReorderSlidesAsync(Guid? tenantId, IReadOnlyList<ReorderSlideItemInput> items, CancellationToken cancellationToken);
    Task<SlideLayerDto> CreateLayerAsync(Guid? tenantId, CreateLayerInput input, CancellationToken cancellationToken);
    Task<SlideLayerDto?> UpdateLayerAsync(Guid? tenantId, Guid layerId, UpdateLayerInput input, CancellationToken cancellationToken);
    Task<bool> DeleteLayerAsync(Guid? tenantId, Guid layerId, CancellationToken cancellationToken);
}

public sealed record SliderConfigDto(
    Guid Id,
    Guid? TenantId,
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
    SliderScrollBehavior ScrollBehavior,
    IReadOnlyList<SlideDto> Slides);

public sealed record SlideDto(
    Guid Id,
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
    IReadOnlyList<SlideLayerDto> Layers,
    IReadOnlyList<SlideHighlightDto> Highlights);

public sealed record SlideLayerDto(
    Guid Id,
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

public sealed record SlideHighlightDto(
    Guid Id,
    string Text,
    string Variant,
    int Order);

public sealed record UpdateSliderConfigInput(
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

public sealed record SlideHighlightInput(
    string Text,
    string Variant,
    int Order);

public sealed record CreateSlideInput(
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
    IReadOnlyList<SlideHighlightInput> Highlights);

public sealed record UpdateSlideInput(
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
    IReadOnlyList<SlideHighlightInput> Highlights);

public sealed record ReorderSlideItemInput(
    Guid SlideId,
    int Order);

public sealed record CreateLayerInput(
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

public sealed record UpdateLayerInput(
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
