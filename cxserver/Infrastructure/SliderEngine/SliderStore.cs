using System.Text.RegularExpressions;
using cxserver.Application.Abstractions;
using cxserver.Domain.SliderEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.SliderEngine;

internal sealed class SliderStore : ISliderStore
{
    private static readonly Regex SafeTextRegex = new("^[^<>]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex YoutubeIdRegex = new("^[a-zA-Z0-9_-]{11}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly ITenantDbContextAccessor _tenantDbContextAccessor;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SliderStore(
        ITenantDbContextAccessor tenantDbContextAccessor,
        ITenantContext tenantContext,
        IDateTimeProvider dateTimeProvider)
    {
        _tenantDbContextAccessor = tenantDbContextAccessor;
        _tenantContext = tenantContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SliderConfigDto> GetOrCreateAsync(Guid? tenantId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);

        var config = await dbContext.SliderConfigs
            .Include(x => x.Slides)
                .ThenInclude(x => x.Layers)
            .Include(x => x.Slides)
                .ThenInclude(x => x.Highlights)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == scope, cancellationToken);

        if (scope.HasValue && (config is null || config.Slides.All(x => x.IsDeleted)))
        {
            // Tenant-scoped slider is optional; use the seeded global slider as read fallback.
            var globalConfig = await dbContext.SliderConfigs
                .Include(x => x.Slides)
                    .ThenInclude(x => x.Layers)
                .Include(x => x.Slides)
                    .ThenInclude(x => x.Highlights)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == null, cancellationToken);

            if (globalConfig is not null)
            {
                return MapConfig(globalConfig);
            }
        }

        if (config is null)
        {
            config = SliderConfig.Create(Guid.NewGuid(), scope, _dateTimeProvider.UtcNow);
            await dbContext.SliderConfigs.AddAsync(config, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return MapConfig(config);
    }

    public async Task<SliderConfigDto> UpdateConfigAsync(Guid? tenantId, UpdateSliderConfigInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);

        config.Update(
            input.IsActive,
            input.HeightMode,
            input.HeightValue,
            input.ContainerMode,
            input.ContentAlignment,
            input.Autoplay,
            input.Loop,
            input.ShowProgress,
            input.ShowNavArrows,
            input.ShowDots,
            input.Parallax,
            input.Particles,
            input.DefaultVariant,
            input.DefaultIntensity,
            input.DefaultDirection,
            input.DefaultBackgroundMode,
            input.ScrollBehavior,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapConfig(config);
    }

    public async Task<SlideDto> CreateSlideAsync(Guid? tenantId, CreateSlideInput input, CancellationToken cancellationToken)
    {
        ValidateSlideInput(input.Title, input.Tagline, input.ActionText, input.ActionHref, input.BackgroundUrl, input.MediaType, input.YoutubeVideoId);

        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);

        var slide = config.AddSlide(
            Guid.NewGuid(),
            input.Order,
            SanitizeText(input.Title),
            SanitizeText(input.Tagline),
            input.ActionText is null ? null : SanitizeText(input.ActionText),
            input.ActionHref,
            input.CtaColor,
            input.Duration,
            input.Direction,
            input.Variant,
            input.Intensity,
            input.BackgroundMode,
            input.ShowOverlay,
            SanitizeText(input.OverlayToken),
            input.BackgroundUrl,
            input.MediaType,
            input.YoutubeVideoId,
            input.IsActive,
            _dateTimeProvider.UtcNow);

        slide.ReplaceHighlights(input.Highlights.Select(x => (SanitizeText(x.Text), SanitizeText(x.Variant), x.Order)).ToList(), _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapSlide(slide);
    }

    public async Task<SlideDto?> UpdateSlideAsync(Guid? tenantId, Guid slideId, UpdateSlideInput input, CancellationToken cancellationToken)
    {
        ValidateSlideInput(input.Title, input.Tagline, input.ActionText, input.ActionHref, input.BackgroundUrl, input.MediaType, input.YoutubeVideoId);

        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);

        if (config.Slides.All(x => x.Id != slideId || x.IsDeleted))
        {
            return null;
        }

        var slide = config.UpdateSlide(
            slideId,
            input.Order,
            SanitizeText(input.Title),
            SanitizeText(input.Tagline),
            input.ActionText is null ? null : SanitizeText(input.ActionText),
            input.ActionHref,
            input.CtaColor,
            input.Duration,
            input.Direction,
            input.Variant,
            input.Intensity,
            input.BackgroundMode,
            input.ShowOverlay,
            SanitizeText(input.OverlayToken),
            input.BackgroundUrl,
            input.MediaType,
            input.YoutubeVideoId,
            input.IsActive,
            _dateTimeProvider.UtcNow);

        slide.ReplaceHighlights(input.Highlights.Select(x => (SanitizeText(x.Text), SanitizeText(x.Variant), x.Order)).ToList(), _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapSlide(slide);
    }

    public async Task<bool> DeleteSlideAsync(Guid? tenantId, Guid slideId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);

        if (config.Slides.All(x => x.Id != slideId || x.IsDeleted))
        {
            return false;
        }

        config.DeleteSlide(slideId, _dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ReorderSlidesAsync(Guid? tenantId, IReadOnlyList<ReorderSlideItemInput> items, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);
        config.ReorderSlides(items.Select(x => (x.SlideId, x.Order)).ToList(), _dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<SlideLayerDto> CreateLayerAsync(Guid? tenantId, CreateLayerInput input, CancellationToken cancellationToken)
    {
        ValidateLayerInput(input.Content, input.MediaUrl, input.PositionX, input.PositionY, input.Width);

        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);
        var slide = config.Slides.FirstOrDefault(x => x.Id == input.SlideId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Slide not found.");

        var layer = slide.AddLayer(
            Guid.NewGuid(),
            input.Order,
            input.Type,
            SanitizeText(input.Content),
            input.MediaUrl,
            input.PositionX,
            input.PositionY,
            input.Width,
            input.AnimationFrom,
            input.AnimationDelay,
            input.AnimationDuration,
            input.AnimationEasing,
            input.ResponsiveVisibility,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapLayer(layer);
    }

    public async Task<SlideLayerDto?> UpdateLayerAsync(Guid? tenantId, Guid layerId, UpdateLayerInput input, CancellationToken cancellationToken)
    {
        ValidateLayerInput(input.Content, input.MediaUrl, input.PositionX, input.PositionY, input.Width);

        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);
        var slide = config.Slides.FirstOrDefault(x => x.Layers.Any(l => l.Id == layerId && !l.IsDeleted) && !x.IsDeleted);
        if (slide is null)
        {
            return null;
        }

        var layer = slide.UpdateLayer(
            layerId,
            input.Order,
            input.Type,
            SanitizeText(input.Content),
            input.MediaUrl,
            input.PositionX,
            input.PositionY,
            input.Width,
            input.AnimationFrom,
            input.AnimationDelay,
            input.AnimationDuration,
            input.AnimationEasing,
            input.ResponsiveVisibility,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapLayer(layer);
    }

    public async Task<bool> DeleteLayerAsync(Guid? tenantId, Guid layerId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scope = ResolveTenantScope(tenantId);
        var config = await EnsureConfigTrackedAsync(dbContext, scope, cancellationToken);
        var slide = config.Slides.FirstOrDefault(x => x.Layers.Any(l => l.Id == layerId && !l.IsDeleted) && !x.IsDeleted);
        if (slide is null)
        {
            return false;
        }

        slide.DeleteLayer(layerId, _dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<SliderConfig> EnsureConfigTrackedAsync(TenantDbContext dbContext, Guid? tenantId, CancellationToken cancellationToken)
    {
        var config = await dbContext.SliderConfigs
            .Include(x => x.Slides)
                .ThenInclude(x => x.Layers)
            .Include(x => x.Slides)
                .ThenInclude(x => x.Highlights)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

        if (config is null)
        {
            config = SliderConfig.Create(Guid.NewGuid(), tenantId, _dateTimeProvider.UtcNow);
            await dbContext.SliderConfigs.AddAsync(config, cancellationToken);
        }

        return config;
    }

    private Guid? ResolveTenantScope(Guid? requestedTenantId)
    {
        if (!requestedTenantId.HasValue)
        {
            return null;
        }

        if (!_tenantContext.TenantId.HasValue || requestedTenantId != _tenantContext.TenantId)
        {
            throw new InvalidOperationException("Tenant scope mismatch.");
        }

        return requestedTenantId;
    }

    private static void ValidateSlideInput(string title, string tagline, string? actionText, string? actionHref, string backgroundUrl, SliderMediaType mediaType, string? youtubeVideoId)
    {
        if (!SafeTextRegex.IsMatch(title) || !SafeTextRegex.IsMatch(tagline))
        {
            throw new InvalidOperationException("Invalid text payload.");
        }

        if (!string.IsNullOrWhiteSpace(actionText) && !SafeTextRegex.IsMatch(actionText))
        {
            throw new InvalidOperationException("Invalid action text.");
        }

        ValidateUrl(backgroundUrl);

        if (!string.IsNullOrWhiteSpace(actionHref))
        {
            ValidateUrl(actionHref);
        }

        if (mediaType == SliderMediaType.Youtube)
        {
            if (string.IsNullOrWhiteSpace(youtubeVideoId) || !YoutubeIdRegex.IsMatch(youtubeVideoId))
            {
                throw new InvalidOperationException("Invalid YouTube video id.");
            }
        }
    }

    private static void ValidateLayerInput(string content, string? mediaUrl, decimal x, decimal y, string width)
    {
        if (!SafeTextRegex.IsMatch(content))
        {
            throw new InvalidOperationException("Invalid layer content.");
        }

        if (!string.IsNullOrWhiteSpace(mediaUrl))
        {
            ValidateUrl(mediaUrl);
        }

        if (x < 0 || x > 100 || y < 0 || y > 100)
        {
            throw new InvalidOperationException("Invalid layer position.");
        }

        if (string.IsNullOrWhiteSpace(width))
        {
            throw new InvalidOperationException("Width is required.");
        }
    }

    private static void ValidateUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Url is required.");
        }

        if (value.Contains("\n", StringComparison.Ordinal) || value.Contains("\r", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Invalid url.");
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var absolute))
        {
            if (absolute.Scheme is not "http" and not "https")
            {
                throw new InvalidOperationException("Unsupported url scheme.");
            }

            return;
        }

        if (!value.StartsWith("/", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Relative url must start with '/'.");
        }
    }

    private static string SanitizeText(string value)
    {
        if (!SafeTextRegex.IsMatch(value))
        {
            throw new InvalidOperationException("Invalid text payload.");
        }

        return value.Trim();
    }

    private static SliderConfigDto MapConfig(SliderConfig config)
    {
        return new SliderConfigDto(
            config.Id,
            config.TenantId,
            config.IsActive,
            config.HeightMode,
            config.HeightValue,
            config.ContainerMode,
            config.ContentAlignment,
            config.Autoplay,
            config.Loop,
            config.ShowProgress,
            config.ShowNavArrows,
            config.ShowDots,
            config.Parallax,
            config.Particles,
            config.DefaultVariant,
            config.DefaultIntensity,
            config.DefaultDirection,
            config.DefaultBackgroundMode,
            config.ScrollBehavior,
            config.Slides.Where(x => !x.IsDeleted).OrderBy(x => x.Order).Select(MapSlide).ToList());
    }

    private static SlideDto MapSlide(Slide slide)
    {
        return new SlideDto(
            slide.Id,
            slide.Order,
            slide.Title,
            slide.Tagline,
            slide.ActionText,
            slide.ActionHref,
            slide.CtaColor,
            slide.Duration,
            slide.Direction,
            slide.Variant,
            slide.Intensity,
            slide.BackgroundMode,
            slide.ShowOverlay,
            slide.OverlayToken,
            slide.BackgroundUrl,
            slide.MediaType,
            slide.YoutubeVideoId,
            slide.IsActive,
            slide.Layers.Where(x => !x.IsDeleted).OrderBy(x => x.Order).Select(MapLayer).ToList(),
            slide.Highlights.Where(x => !x.IsDeleted).OrderBy(x => x.Order).Select(MapHighlight).ToList());
    }

    private static SlideLayerDto MapLayer(SlideLayer layer)
    {
        return new SlideLayerDto(
            layer.Id,
            layer.Order,
            layer.Type,
            layer.Content,
            layer.MediaUrl,
            layer.PositionX,
            layer.PositionY,
            layer.Width,
            layer.AnimationFrom,
            layer.AnimationDelay,
            layer.AnimationDuration,
            layer.AnimationEasing,
            layer.ResponsiveVisibility);
    }

    private static SlideHighlightDto MapHighlight(SlideHighlight highlight)
    {
        return new SlideHighlightDto(highlight.Id, highlight.Text, highlight.Variant, highlight.Order);
    }
}
