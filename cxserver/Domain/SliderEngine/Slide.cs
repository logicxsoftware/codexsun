using cxserver.Domain.Common;

namespace cxserver.Domain.SliderEngine;

public sealed class Slide : ISoftDeletable
{
    private readonly List<SlideLayer> _layers;
    private readonly List<SlideHighlight> _highlights;

    private Slide(
        Guid id,
        Guid sliderConfigId,
        int order,
        string title,
        string tagline,
        string? actionText,
        string? actionHref,
        SliderCtaColor ctaColor,
        int duration,
        SliderDirection direction,
        SliderVariant variant,
        SliderIntensity intensity,
        SliderBackgroundMode backgroundMode,
        bool showOverlay,
        string overlayToken,
        string backgroundUrl,
        SliderMediaType mediaType,
        string? youtubeVideoId,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        Id = id;
        SliderConfigId = sliderConfigId;
        Order = order;
        Title = title;
        Tagline = tagline;
        ActionText = actionText;
        ActionHref = actionHref;
        CtaColor = ctaColor;
        Duration = duration;
        Direction = direction;
        Variant = variant;
        Intensity = intensity;
        BackgroundMode = backgroundMode;
        ShowOverlay = showOverlay;
        OverlayToken = overlayToken;
        BackgroundUrl = backgroundUrl;
        MediaType = mediaType;
        YoutubeVideoId = youtubeVideoId;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
        _layers = new List<SlideLayer>();
        _highlights = new List<SlideHighlight>();
    }

    private Slide()
    {
        Title = string.Empty;
        Tagline = string.Empty;
        OverlayToken = string.Empty;
        BackgroundUrl = string.Empty;
        _layers = new List<SlideLayer>();
        _highlights = new List<SlideHighlight>();
    }

    public Guid Id { get; private set; }
    public Guid SliderConfigId { get; private set; }
    public int Order { get; private set; }
    public string Title { get; private set; }
    public string Tagline { get; private set; }
    public string? ActionText { get; private set; }
    public string? ActionHref { get; private set; }
    public SliderCtaColor CtaColor { get; private set; }
    public int Duration { get; private set; }
    public SliderDirection Direction { get; private set; }
    public SliderVariant Variant { get; private set; }
    public SliderIntensity Intensity { get; private set; }
    public SliderBackgroundMode BackgroundMode { get; private set; }
    public bool ShowOverlay { get; private set; }
    public string OverlayToken { get; private set; }
    public string BackgroundUrl { get; private set; }
    public SliderMediaType MediaType { get; private set; }
    public string? YoutubeVideoId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public IReadOnlyCollection<SlideLayer> Layers => _layers.AsReadOnly();
    public IReadOnlyCollection<SlideHighlight> Highlights => _highlights.AsReadOnly();

    internal static Slide Create(
        Guid id,
        Guid sliderConfigId,
        int order,
        string title,
        string tagline,
        string? actionText,
        string? actionHref,
        SliderCtaColor ctaColor,
        int duration,
        SliderDirection direction,
        SliderVariant variant,
        SliderIntensity intensity,
        SliderBackgroundMode backgroundMode,
        bool showOverlay,
        string overlayToken,
        string backgroundUrl,
        SliderMediaType mediaType,
        string? youtubeVideoId,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        Validate(title, tagline, overlayToken, backgroundUrl, duration);
        return new Slide(
            id,
            sliderConfigId,
            order,
            title.Trim(),
            tagline.Trim(),
            actionText?.Trim(),
            actionHref?.Trim(),
            ctaColor,
            duration,
            direction,
            variant,
            intensity,
            backgroundMode,
            showOverlay,
            overlayToken.Trim(),
            backgroundUrl.Trim(),
            mediaType,
            youtubeVideoId?.Trim(),
            isActive,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    internal void Update(
        int order,
        string title,
        string tagline,
        string? actionText,
        string? actionHref,
        SliderCtaColor ctaColor,
        int duration,
        SliderDirection direction,
        SliderVariant variant,
        SliderIntensity intensity,
        SliderBackgroundMode backgroundMode,
        bool showOverlay,
        string overlayToken,
        string backgroundUrl,
        SliderMediaType mediaType,
        string? youtubeVideoId,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        Validate(title, tagline, overlayToken, backgroundUrl, duration);
        Order = order;
        Title = title.Trim();
        Tagline = tagline.Trim();
        ActionText = actionText?.Trim();
        ActionHref = actionHref?.Trim();
        CtaColor = ctaColor;
        Duration = duration;
        Direction = direction;
        Variant = variant;
        Intensity = intensity;
        BackgroundMode = backgroundMode;
        ShowOverlay = showOverlay;
        OverlayToken = overlayToken.Trim();
        BackgroundUrl = backgroundUrl.Trim();
        MediaType = mediaType;
        YoutubeVideoId = youtubeVideoId?.Trim();
        IsActive = isActive;
        UpdatedAtUtc = nowUtc;
    }

    internal void Reorder(int order, DateTimeOffset nowUtc)
    {
        Order = order;
        UpdatedAtUtc = nowUtc;
    }

    public SlideLayer AddLayer(
        Guid id,
        int order,
        SliderLayerType type,
        string content,
        string? mediaUrl,
        decimal positionX,
        decimal positionY,
        string width,
        SliderLayerAnimationFrom animationFrom,
        int animationDelay,
        int animationDuration,
        string animationEasing,
        string responsiveVisibility,
        DateTimeOffset nowUtc)
    {
        EnsureLayerOrderUnique(order, null);
        var layer = SlideLayer.Create(
            id,
            Id,
            order,
            type,
            content,
            mediaUrl,
            positionX,
            positionY,
            width,
            animationFrom,
            animationDelay,
            animationDuration,
            animationEasing,
            responsiveVisibility,
            nowUtc);
        _layers.Add(layer);
        NormalizeLayerOrder(nowUtc);
        UpdatedAtUtc = nowUtc;
        return layer;
    }

    public SlideLayer UpdateLayer(
        Guid layerId,
        int order,
        SliderLayerType type,
        string content,
        string? mediaUrl,
        decimal positionX,
        decimal positionY,
        string width,
        SliderLayerAnimationFrom animationFrom,
        int animationDelay,
        int animationDuration,
        string animationEasing,
        string responsiveVisibility,
        DateTimeOffset nowUtc)
    {
        var layer = _layers.FirstOrDefault(x => x.Id == layerId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Layer not found.");
        EnsureLayerOrderUnique(order, layerId);
        layer.Update(order, type, content, mediaUrl, positionX, positionY, width, animationFrom, animationDelay, animationDuration, animationEasing, responsiveVisibility, nowUtc);
        NormalizeLayerOrder(nowUtc);
        UpdatedAtUtc = nowUtc;
        return layer;
    }

    public void DeleteLayer(Guid layerId, DateTimeOffset nowUtc)
    {
        var layer = _layers.FirstOrDefault(x => x.Id == layerId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Layer not found.");
        layer.Delete(nowUtc);
        NormalizeLayerOrder(nowUtc);
        UpdatedAtUtc = nowUtc;
    }

    public void ReorderLayers(IReadOnlyList<(Guid LayerId, int Order)> orderMap, DateTimeOffset nowUtc)
    {
        foreach (var mapping in orderMap)
        {
            var layer = _layers.FirstOrDefault(x => x.Id == mapping.LayerId && !x.IsDeleted)
                ?? throw new InvalidOperationException("Invalid layer id.");
            layer.Reorder(mapping.Order, nowUtc);
        }

        NormalizeLayerOrder(nowUtc);
        UpdatedAtUtc = nowUtc;
    }

    public void ReplaceHighlights(IReadOnlyList<(string Text, string Variant, int Order)> highlights, DateTimeOffset nowUtc)
    {
        var existingActive = _highlights.Where(x => !x.IsDeleted).OrderBy(x => x.Order).ToList();
        var expectedByOrder = highlights.ToDictionary(x => x.Order, x => x);

        foreach (var existing in existingActive)
        {
            if (!expectedByOrder.TryGetValue(existing.Order, out var target))
            {
                existing.Delete(nowUtc);
                continue;
            }

            existing.Update(target.Text, target.Variant, target.Order, nowUtc);
        }

        var existingOrders = _highlights.Where(x => !x.IsDeleted).Select(x => x.Order).ToHashSet();
        foreach (var highlight in highlights.OrderBy(x => x.Order))
        {
            if (existingOrders.Contains(highlight.Order))
            {
                continue;
            }

            _highlights.Add(SlideHighlight.Create(Guid.NewGuid(), Id, highlight.Text, highlight.Variant, highlight.Order, nowUtc));
        }

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

    private void EnsureLayerOrderUnique(int order, Guid? layerId)
    {
        if (_layers.Any(x => !x.IsDeleted && x.Order == order && x.Id != layerId))
        {
            throw new InvalidOperationException("Layer order must be unique.");
        }
    }

    private void NormalizeLayerOrder(DateTimeOffset nowUtc)
    {
        var ordered = _layers.Where(x => !x.IsDeleted).OrderBy(x => x.Order).ThenBy(x => x.CreatedAtUtc).ToList();
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].Reorder(index, nowUtc);
        }
    }

    private static void Validate(string title, string tagline, string overlayToken, string backgroundUrl, int duration)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(tagline))
        {
            throw new ArgumentException("Tagline is required.", nameof(tagline));
        }

        if (string.IsNullOrWhiteSpace(overlayToken))
        {
            throw new ArgumentException("Overlay token is required.", nameof(overlayToken));
        }

        if (string.IsNullOrWhiteSpace(backgroundUrl))
        {
            throw new ArgumentException("Background url is required.", nameof(backgroundUrl));
        }

        if (duration < 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
    }
}
