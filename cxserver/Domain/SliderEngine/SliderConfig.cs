using cxserver.Domain.Common;

namespace cxserver.Domain.SliderEngine;

public sealed class SliderConfig : AggregateRoot, ISoftDeletable
{
    private readonly List<Slide> _slides;

    private SliderConfig(
        Guid id,
        Guid? tenantId,
        bool isActive,
        SliderHeightMode heightMode,
        int heightValue,
        SliderContainerMode containerMode,
        SliderContentAlignment contentAlignment,
        bool autoplay,
        bool loop,
        bool showProgress,
        bool showNavArrows,
        bool showDots,
        bool parallax,
        bool particles,
        SliderVariant defaultVariant,
        SliderIntensity defaultIntensity,
        SliderDirection defaultDirection,
        SliderBackgroundMode defaultBackgroundMode,
        SliderScrollBehavior scrollBehavior,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        TenantId = tenantId;
        IsActive = isActive;
        HeightMode = heightMode;
        HeightValue = heightValue;
        ContainerMode = containerMode;
        ContentAlignment = contentAlignment;
        Autoplay = autoplay;
        Loop = loop;
        ShowProgress = showProgress;
        ShowNavArrows = showNavArrows;
        ShowDots = showDots;
        Parallax = parallax;
        Particles = particles;
        DefaultVariant = defaultVariant;
        DefaultIntensity = defaultIntensity;
        DefaultDirection = defaultDirection;
        DefaultBackgroundMode = defaultBackgroundMode;
        ScrollBehavior = scrollBehavior;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
        _slides = new List<Slide>();
    }

    private SliderConfig() : base(Guid.NewGuid())
    {
        _slides = new List<Slide>();
    }

    public Guid? TenantId { get; private set; }
    public bool IsActive { get; private set; }
    public SliderHeightMode HeightMode { get; private set; }
    public int HeightValue { get; private set; }
    public SliderContainerMode ContainerMode { get; private set; }
    public SliderContentAlignment ContentAlignment { get; private set; }
    public bool Autoplay { get; private set; }
    public bool Loop { get; private set; }
    public bool ShowProgress { get; private set; }
    public bool ShowNavArrows { get; private set; }
    public bool ShowDots { get; private set; }
    public bool Parallax { get; private set; }
    public bool Particles { get; private set; }
    public SliderVariant DefaultVariant { get; private set; }
    public SliderIntensity DefaultIntensity { get; private set; }
    public SliderDirection DefaultDirection { get; private set; }
    public SliderBackgroundMode DefaultBackgroundMode { get; private set; }
    public SliderScrollBehavior ScrollBehavior { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public IReadOnlyCollection<Slide> Slides => _slides.AsReadOnly();

    public static SliderConfig Create(Guid id, Guid? tenantId, DateTimeOffset nowUtc)
    {
        return new SliderConfig(
            id,
            tenantId,
            true,
            SliderHeightMode.Fullscreen,
            100,
            SliderContainerMode.Containered,
            SliderContentAlignment.Center,
            true,
            true,
            true,
            true,
            true,
            false,
            false,
            SliderVariant.Default,
            SliderIntensity.Medium,
            SliderDirection.Fade,
            SliderBackgroundMode.Normal,
            SliderScrollBehavior.Normal,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Update(
        bool isActive,
        SliderHeightMode heightMode,
        int heightValue,
        SliderContainerMode containerMode,
        SliderContentAlignment contentAlignment,
        bool autoplay,
        bool loop,
        bool showProgress,
        bool showNavArrows,
        bool showDots,
        bool parallax,
        bool particles,
        SliderVariant defaultVariant,
        SliderIntensity defaultIntensity,
        SliderDirection defaultDirection,
        SliderBackgroundMode defaultBackgroundMode,
        SliderScrollBehavior scrollBehavior,
        DateTimeOffset nowUtc)
    {
        if (heightValue <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heightValue));
        }

        IsActive = isActive;
        HeightMode = heightMode;
        HeightValue = heightValue;
        ContainerMode = containerMode;
        ContentAlignment = contentAlignment;
        Autoplay = autoplay;
        Loop = loop;
        ShowProgress = showProgress;
        ShowNavArrows = showNavArrows;
        ShowDots = showDots;
        Parallax = parallax;
        Particles = particles;
        DefaultVariant = defaultVariant;
        DefaultIntensity = defaultIntensity;
        DefaultDirection = defaultDirection;
        DefaultBackgroundMode = defaultBackgroundMode;
        ScrollBehavior = scrollBehavior;
        UpdatedAtUtc = nowUtc;
    }

    public Slide AddSlide(
        Guid id,
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
        EnsureSlideOrderUnique(order, null);
        var slide = Slide.Create(
            id,
            Id,
            order,
            title,
            tagline,
            actionText,
            actionHref,
            ctaColor,
            duration,
            direction,
            variant,
            intensity,
            backgroundMode,
            showOverlay,
            overlayToken,
            backgroundUrl,
            mediaType,
            youtubeVideoId,
            isActive,
            nowUtc);

        _slides.Add(slide);
        NormalizeSlideOrder(nowUtc);
        UpdatedAtUtc = nowUtc;
        return slide;
    }

    public Slide UpdateSlide(
        Guid slideId,
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
        var slide = _slides.FirstOrDefault(x => x.Id == slideId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Slide not found.");
        EnsureSlideOrderUnique(order, slideId);

        slide.Update(
            order,
            title,
            tagline,
            actionText,
            actionHref,
            ctaColor,
            duration,
            direction,
            variant,
            intensity,
            backgroundMode,
            showOverlay,
            overlayToken,
            backgroundUrl,
            mediaType,
            youtubeVideoId,
            isActive,
            nowUtc);

        NormalizeSlideOrder(nowUtc);
        UpdatedAtUtc = nowUtc;
        return slide;
    }

    public void DeleteSlide(Guid slideId, DateTimeOffset nowUtc)
    {
        var slide = _slides.FirstOrDefault(x => x.Id == slideId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Slide not found.");
        slide.Delete(nowUtc);
        NormalizeSlideOrder(nowUtc);
        UpdatedAtUtc = nowUtc;
    }

    public void ReorderSlides(IReadOnlyList<(Guid SlideId, int Order)> orderMap, DateTimeOffset nowUtc)
    {
        var distinct = orderMap.Select(x => x.SlideId).Distinct().Count();
        if (distinct != orderMap.Count)
        {
            throw new InvalidOperationException("Duplicate slide ids.");
        }

        foreach (var mapping in orderMap)
        {
            var slide = _slides.FirstOrDefault(x => x.Id == mapping.SlideId && !x.IsDeleted)
                ?? throw new InvalidOperationException("Invalid slide id.");
            slide.Reorder(mapping.Order, nowUtc);
        }

        NormalizeSlideOrder(nowUtc);
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

    private void EnsureSlideOrderUnique(int order, Guid? slideId)
    {
        if (_slides.Any(x => !x.IsDeleted && x.Order == order && x.Id != slideId))
        {
            throw new InvalidOperationException("Slide order must be unique.");
        }
    }

    private void NormalizeSlideOrder(DateTimeOffset nowUtc)
    {
        var ordered = _slides.Where(x => !x.IsDeleted).OrderBy(x => x.Order).ThenBy(x => x.CreatedAtUtc).ToList();
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].Reorder(index, nowUtc);
        }
    }
}
