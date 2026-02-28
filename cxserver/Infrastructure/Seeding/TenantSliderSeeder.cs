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

        var slide1 = EnsureSlide(config, 0, "Build Smarter SaaS Products with Codexsun", "Enterprise-grade CRM, ERP, HRMS and automation systems designed for scale.", "Get Started", "/signup", SliderCtaColor.Primary, 6500, SliderDirection.Left, SliderVariant.Saas, SliderIntensity.Medium, SliderBackgroundMode.Cinematic, true, "muted/70", "https://images.unsplash.com/photo-1551434678-e076c223a692?w=1920", SliderMediaType.Image, null, now);
        slide1.ReplaceHighlights(
            [
                ("Multi-Tenant Ready", "success", 0),
                ("API First Architecture", "primary", 1),
                ("Cloud Scalable", "glass", 2),
            ],
            now);
        EnsureLayer(slide1, 0, SliderLayerType.Image, "Floating dashboard image", "https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=900", 56, 22, "34%", SliderLayerAnimationFrom.Right, 200, 650, "ease-out", "desktop", now);
        EnsureLayer(slide1, 1, SliderLayerType.Badge, "Trusted by 200+ Businesses", null, 10, 12, "220px", SliderLayerAnimationFrom.Top, 80, 550, "ease-out", "all", now);
        EnsureLayer(slide1, 2, SliderLayerType.Button, "Get Started", null, 10, 64, "180px", SliderLayerAnimationFrom.Zoom, 280, 600, "ease-out", "all", now);
        EnsureLayer(slide1, 3, SliderLayerType.Custom, "preloadNext=true;lazyMedia=true;gpuAcceleration=true;optimizedRendering=true", null, 0, 0, "0px", SliderLayerAnimationFrom.Fade, 0, 200, "linear", "desktop", now);

        var slide2 = EnsureSlide(config, 1, "Codexsun CRM", "Track leads, automate pipelines, and close deals faster.", "Explore CRM", "/products/crm", SliderCtaColor.Success, 6200, SliderDirection.Right, SliderVariant.Classic, SliderIntensity.Low, SliderBackgroundMode.Parallax, true, "background/60", "https://images.unsplash.com/photo-1556740749-887f6717d7e4?w=1920", SliderMediaType.Image, null, now);
        slide2.ReplaceHighlights(
            [
                ("Pipeline Automation", "primary", 0),
                ("Lead Intelligence", "success", 1),
            ],
            now);
        EnsureLayer(slide2, 0, SliderLayerType.Image, "Analytics card", "https://images.unsplash.com/photo-1551281044-8a41b0c84ff4?w=640", 58, 20, "28%", SliderLayerAnimationFrom.Left, 140, 600, "ease-out", "desktop", now);
        EnsureLayer(slide2, 1, SliderLayerType.Image, "Floating chart", "https://images.unsplash.com/photo-1543286386-713bdd548da4?w=640", 64, 58, "24%", SliderLayerAnimationFrom.Bottom, 220, 620, "ease-out", "desktop", now);
        EnsureLayer(slide2, 2, SliderLayerType.Button, "See CRM Features", null, 10, 66, "190px", SliderLayerAnimationFrom.Zoom, 260, 560, "ease-out", "all", now);

        var slide3 = EnsureSlide(config, 2, "Full-Scale ERP for Growing Enterprises", "Finance, inventory, HR, procurement — unified under one intelligent system.", "Explore ERP", "/products/erp", SliderCtaColor.Secondary, 6800, SliderDirection.Fade, SliderVariant.Luxury, SliderIntensity.Medium, SliderBackgroundMode.Normal, true, "foreground/20", "/assets/techmedia/videos/erp-bg.mp4", SliderMediaType.Video, null, now);
        slide3.ReplaceHighlights(
            [
                ("Unified Operations", "secondary", 0),
                ("Enterprise Modules", "primary", 1),
            ],
            now);
        EnsureLayer(slide3, 0, SliderLayerType.Image, "ERP dashboard image overlay", "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=900", 56, 20, "34%", SliderLayerAnimationFrom.Right, 180, 640, "ease-out", "desktop", now);
        EnsureLayer(slide3, 1, SliderLayerType.Badge, "KPI +18% Efficiency", null, 10, 14, "190px", SliderLayerAnimationFrom.Top, 80, 500, "ease-out", "all", now);
        EnsureLayer(slide3, 2, SliderLayerType.Text, "Enterprise Operations", null, 10, 24, "46%", SliderLayerAnimationFrom.Fade, 120, 560, "ease-out", "all", now);

        var slide4 = EnsureSlide(config, 3, "Modern HRMS Built for Teams", "Attendance, payroll, onboarding and compliance simplified.", "Discover HRMS", "/products/hrms", SliderCtaColor.Warning, 6400, SliderDirection.Left, SliderVariant.Industrial, SliderIntensity.Medium, SliderBackgroundMode.ThreeD, true, "card/60", "https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=1920", SliderMediaType.Image, null, now);
        slide4.ReplaceHighlights(
            [
                ("Payroll Automation", "warning", 0),
                ("Team Performance", "info", 1),
            ],
            now);
        EnsureLayer(slide4, 0, SliderLayerType.Image, "Employee avatar cluster", "https://images.unsplash.com/photo-1542744173-8e7e53415bb0?w=640", 58, 18, "30%", SliderLayerAnimationFrom.Right, 160, 620, "ease-out", "desktop", now);
        EnsureLayer(slide4, 1, SliderLayerType.Badge, "Payroll 99.9% Accuracy", null, 10, 16, "210px", SliderLayerAnimationFrom.Zoom, 120, 520, "ease-out", "all", now);
        EnsureLayer(slide4, 2, SliderLayerType.Custom, "Animated underline effect", null, 10, 60, "180px", SliderLayerAnimationFrom.Left, 260, 500, "ease-out", "all", now);

        var slide5 = EnsureSlide(config, 4, "Smart POS & Retail Automation", "Real-time billing, inventory sync and sales intelligence.", "See POS", "/products/pos", SliderCtaColor.Danger, 7000, SliderDirection.Fade, SliderVariant.Cinematic, SliderIntensity.High, SliderBackgroundMode.Cinematic, true, "background/50", "https://www.youtube.com/watch?v=jEiGVbT0wVI", SliderMediaType.Youtube, "jEiGVbT0wVI", now);
        slide5.ReplaceHighlights(
            [
                ("Retail Ready", "danger", 0),
                ("Live Billing", "primary", 1),
            ],
            now);
        EnsureLayer(slide5, 0, SliderLayerType.Image, "Product card stack animation", "https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=640", 60, 20, "26%", SliderLayerAnimationFrom.Right, 200, 620, "ease-out", "desktop", now);
        EnsureLayer(slide5, 1, SliderLayerType.Badge, "Sales Counter +245", null, 10, 16, "180px", SliderLayerAnimationFrom.Top, 100, 520, "ease-out", "all", now);
        EnsureLayer(slide5, 2, SliderLayerType.Button, "Start Selling", null, 10, 66, "170px", SliderLayerAnimationFrom.Zoom, 260, 560, "ease-out", "all", now);

        var slide6 = EnsureSlide(config, 5, "Custom SaaS Solutions for Startups & Enterprises", "From MVP to enterprise platform — Codexsun builds scalable software.", "Build with Codexsun", "/products/custom-saas", SliderCtaColor.Primary, 7200, SliderDirection.Right, SliderVariant.Saas, SliderIntensity.High, SliderBackgroundMode.Parallax, true, "muted/70", "https://images.unsplash.com/photo-1518779578993-ec3579fee39f?w=1920", SliderMediaType.Image, null, now);
        slide6.ReplaceHighlights(
            [
                ("MVP to Scale", "primary", 0),
                ("Dedicated Engineering", "success", 1),
            ],
            now);
        EnsureLayer(slide6, 0, SliderLayerType.Custom, "Code snippet overlay", null, 10, 12, "240px", SliderLayerAnimationFrom.Fade, 120, 560, "ease-out", "all", now);
        EnsureLayer(slide6, 1, SliderLayerType.Image, "Floating device mockups", "https://images.unsplash.com/photo-1517336714739-489689fd1ca8?w=640", 58, 22, "30%", SliderLayerAnimationFrom.Right, 180, 620, "ease-out", "desktop", now);
        EnsureLayer(slide6, 2, SliderLayerType.Badge, "Startup + Enterprise", null, 10, 62, "190px", SliderLayerAnimationFrom.Bottom, 260, 540, "ease-out", "all", now);

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
}
