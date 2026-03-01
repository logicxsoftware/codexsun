using cxserver.Domain.SliderEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantSliderSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantSliderSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var config = await dbContext.SliderConfigs
            .Include(x => x.Slides)
                .ThenInclude(x => x.Layers)
            .Include(x => x.Slides)
                .ThenInclude(x => x.Highlights)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == null, cancellationToken);

        if (config is null)
        {
            config = SliderConfig.Create(Guid.NewGuid(), null, now);
            await dbContext.SliderConfigs.AddAsync(config, cancellationToken);
        }

        config.Update(
            true,
            SliderHeightMode.Fullscreen,
            100,
            SliderContainerMode.Containered,
            SliderContentAlignment.Left,
            true,
            true,
            true,
            true,
            true,
            true,
            false,
            SliderVariant.Saas,
            SliderIntensity.Medium,
            SliderDirection.Left,
            SliderBackgroundMode.Cinematic,
            SliderScrollBehavior.Fade,
            now);

        var slide1 = EnsureSlide(config, 0, "Laptops and Desktops for Every Need", "Explore premium laptops, custom desktops, and office PCs with trusted local service in Tiruppur.", "Shop Computers", "/products", SliderCtaColor.Primary, 6500, SliderDirection.Left, SliderVariant.Classic, SliderIntensity.Medium, SliderBackgroundMode.Normal, true, "muted/70", "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=1920", SliderMediaType.Image, null, now);
        slide1.ReplaceHighlights(
            [
                ("Business and Gaming Rigs", "primary", 0),
                ("Authorised Brands", "success", 1),
                ("In-store Support", "info", 2),
            ],
            now);
        EnsureLayer(slide1, 0, SliderLayerType.Image, "Laptop display setup", "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=900", 58, 20, "32%", SliderLayerAnimationFrom.Right, 180, 620, "ease-out", "desktop", now);
        EnsureLayer(slide1, 1, SliderLayerType.Badge, "Top Laptop Deals", null, 10, 12, "180px", SliderLayerAnimationFrom.Top, 80, 520, "ease-out", "all", now);
        EnsureLayer(slide1, 2, SliderLayerType.Button, "View Collection", null, 10, 64, "180px", SliderLayerAnimationFrom.Zoom, 260, 580, "ease-out", "all", now);
        EnsureLayer(slide1, 3, SliderLayerType.Custom, "preloadNext=true;lazyMedia=true;gpuAcceleration=true;optimizedRendering=true", null, 0, 0, "0px", SliderLayerAnimationFrom.Fade, 0, 200, "linear", "desktop", now);

        var slide2 = EnsureSlide(config, 1, "Custom Gaming PCs Built to Perform", "High FPS builds with RTX graphics, liquid cooling options, and clean cable management.", "Build My Gaming PC", "/contact", SliderCtaColor.Danger, 6200, SliderDirection.Right, SliderVariant.Cinematic, SliderIntensity.High, SliderBackgroundMode.Cinematic, true, "background/60", "https://images.unsplash.com/photo-1587202372634-32705e3bf49c?w=1920", SliderMediaType.Image, null, now);
        slide2.ReplaceHighlights(
            [
                ("RTX and Ryzen Configs", "danger", 0),
                ("Stress Tested Builds", "primary", 1),
            ],
            now);
        EnsureLayer(slide2, 0, SliderLayerType.Image, "Gaming rig with RGB", "https://images.unsplash.com/photo-1591799265444-d66432b91588?w=640", 60, 22, "28%", SliderLayerAnimationFrom.Left, 140, 600, "ease-out", "desktop", now);
        EnsureLayer(slide2, 1, SliderLayerType.Badge, "Free Setup and Benchmark", null, 10, 16, "230px", SliderLayerAnimationFrom.Top, 120, 520, "ease-out", "all", now);
        EnsureLayer(slide2, 2, SliderLayerType.Button, "Start Build", null, 10, 66, "170px", SliderLayerAnimationFrom.Zoom, 260, 560, "ease-out", "all", now);

        var slide3 = EnsureSlide(config, 2, "Networking and Wi-Fi Solutions", "Routers, switches, access points, and structured cabling for homes and offices.", "Request Site Visit", "/contact", SliderCtaColor.Info, 6400, SliderDirection.Left, SliderVariant.Industrial, SliderIntensity.Medium, SliderBackgroundMode.Parallax, true, "foreground/20", "https://images.unsplash.com/photo-1544197150-b99a580bb7a8?w=1920", SliderMediaType.Image, null, now);
        slide3.ReplaceHighlights(
            [
                ("Secure Office Networks", "primary", 0),
                ("Installation and Support", "success", 1),
            ],
            now);
        EnsureLayer(slide3, 0, SliderLayerType.Image, "Network rack setup", "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=900", 57, 20, "32%", SliderLayerAnimationFrom.Right, 180, 620, "ease-out", "desktop", now);
        EnsureLayer(slide3, 1, SliderLayerType.Badge, "Business Internet Ready", null, 10, 14, "220px", SliderLayerAnimationFrom.Top, 80, 500, "ease-out", "all", now);
        EnsureLayer(slide3, 2, SliderLayerType.Button, "Talk to Expert", null, 10, 66, "170px", SliderLayerAnimationFrom.Zoom, 260, 560, "ease-out", "all", now);

        var slide4 = EnsureSlide(config, 3, "Monitors, Printers and Accessories", "Complete your workspace with monitors, printers, keyboards, mice, SSDs, and UPS systems.", "Shop Accessories", "/products", SliderCtaColor.Warning, 6200, SliderDirection.Fade, SliderVariant.Luxury, SliderIntensity.Low, SliderBackgroundMode.Normal, true, "card/60", "https://images.unsplash.com/photo-1527443224154-c4cd301b7a5f?w=1920", SliderMediaType.Image, null, now);
        slide4.ReplaceHighlights(
            [
                ("Bulk Office Supply", "warning", 0),
                ("Original Components", "secondary", 1),
            ],
            now);
        EnsureLayer(slide4, 0, SliderLayerType.Image, "Office peripherals setup", "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=640", 59, 20, "30%", SliderLayerAnimationFrom.Right, 160, 620, "ease-out", "desktop", now);
        EnsureLayer(slide4, 1, SliderLayerType.Badge, "Same Day Dispatch", null, 10, 16, "170px", SliderLayerAnimationFrom.Zoom, 120, 520, "ease-out", "all", now);
        EnsureLayer(slide4, 2, SliderLayerType.Button, "Browse Products", null, 10, 66, "175px", SliderLayerAnimationFrom.Left, 260, 500, "ease-out", "all", now);

        var slide5 = EnsureSlide(config, 4, "Service Center for Repairs and Upgrades", "Laptop repair, desktop service, data backup, OS installs, and performance tuning by technicians.", "Book Service", "/web-contacts", SliderCtaColor.Success, 6800, SliderDirection.Left, SliderVariant.Saas, SliderIntensity.Medium, SliderBackgroundMode.ThreeD, true, "background/50", "https://images.unsplash.com/photo-1518770660439-4636190af475?w=1920", SliderMediaType.Image, null, now);
        slide5.ReplaceHighlights(
            [
                ("Fast Turnaround", "success", 0),
                ("Genuine Spare Parts", "primary", 1),
            ],
            now);
        EnsureLayer(slide5, 0, SliderLayerType.Image, "Repair bench", "https://images.unsplash.com/photo-1580894908361-967195033215?w=640", 58, 20, "28%", SliderLayerAnimationFrom.Right, 200, 620, "ease-out", "desktop", now);
        EnsureLayer(slide5, 1, SliderLayerType.Badge, "Pickup and Drop Available", null, 10, 16, "240px", SliderLayerAnimationFrom.Top, 100, 520, "ease-out", "all", now);
        EnsureLayer(slide5, 2, SliderLayerType.Button, "Contact Service Team", null, 10, 66, "220px", SliderLayerAnimationFrom.Zoom, 260, 560, "ease-out", "all", now);

        RemoveSlidesOutsideRange(config, minOrder: 0, maxOrder: 4, now);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static Slide EnsureSlide(
        SliderConfig config,
        int order,
        string title,
        string tagline,
        string actionText,
        string actionHref,
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
        string? youtubeId,
        DateTimeOffset now)
    {
        var existing = config.Slides.FirstOrDefault(x => x.Order == order && !x.IsDeleted);
        if (existing is null)
        {
            return config.AddSlide(Guid.NewGuid(), order, title, tagline, actionText, actionHref, ctaColor, duration, direction, variant, intensity, backgroundMode, showOverlay, overlayToken, backgroundUrl, mediaType, youtubeId, true, now);
        }

        return config.UpdateSlide(existing.Id, order, title, tagline, actionText, actionHref, ctaColor, duration, direction, variant, intensity, backgroundMode, showOverlay, overlayToken, backgroundUrl, mediaType, youtubeId, true, now);
    }

    private static SlideLayer EnsureLayer(
        Slide slide,
        int order,
        SliderLayerType type,
        string content,
        string? mediaUrl,
        decimal x,
        decimal y,
        string width,
        SliderLayerAnimationFrom from,
        int delay,
        int duration,
        string easing,
        string visibility,
        DateTimeOffset now)
    {
        var existing = slide.Layers.FirstOrDefault(x => x.Order == order && !x.IsDeleted);
        if (existing is null)
        {
            return slide.AddLayer(Guid.NewGuid(), order, type, content, mediaUrl, x, y, width, from, delay, duration, easing, visibility, now);
        }

        return slide.UpdateLayer(existing.Id, order, type, content, mediaUrl, x, y, width, from, delay, duration, easing, visibility, now);
    }

    private static void RemoveSlidesOutsideRange(SliderConfig config, int minOrder, int maxOrder, DateTimeOffset now)
    {
        var staleSlideIds = config.Slides
            .Where(x => !x.IsDeleted && (x.Order < minOrder || x.Order > maxOrder))
            .Select(x => x.Id)
            .ToList();

        foreach (var slideId in staleSlideIds)
        {
            config.DeleteSlide(slideId, now);
        }
    }
}

