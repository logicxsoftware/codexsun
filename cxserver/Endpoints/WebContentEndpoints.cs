using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Application.Features.MenuEngine.Queries.GetRenderMenus;
using cxserver.Application.Features.WebEngine.Queries.GetPublishedPage;
using MediatR;

namespace cxserver.Endpoints;

public static class WebContentEndpoints
{
    public static RouteGroupBuilder MapWebContentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/web");

        group.MapGet("/{slug}", async (string slug, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetPublishedPageQuery(slug), cancellationToken);
            return response is null ? Results.NotFound() : Results.Ok(response);
        });

        app.MapGet("/api/home-data", async (
            ISender sender,
            ISliderStore sliderStore,
            IWebsiteNavigationStore navigationStore,
            ITenantContext tenantContext,
            CancellationToken cancellationToken) =>
        {
            var page = await sender.Send(new GetPublishedPageQuery("home"), cancellationToken);
            if (page is null)
            {
                return Results.NotFound();
            }

            var slider = await sliderStore.GetOrCreateAsync(tenantContext.TenantId, cancellationToken);
            var navigation = await navigationStore.GetWebNavigationConfigAsync(tenantContext.TenantId, cancellationToken);
            var footer = await navigationStore.GetFooterConfigAsync(tenantContext.TenantId, cancellationToken);
            var menus = await sender.Send(new GetRenderMenusQuery(false), cancellationToken);

            var heroSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.Hero);
            var aboutSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.About);
            var statsSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.Stats);
            var catalogSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.Catalog);
            var whyChooseUsSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.WhyChooseUs);
            var brandSliderSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.BrandSlider);
            var featuresSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.Features);
            var callToActionSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.CallToAction);
            var locationSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.Location);
            var newsletterSection = page.Sections.FirstOrDefault(x => x.SectionType == Domain.WebEngine.SectionType.Newsletter);

            var hero = heroSection?.SectionData;
            var about = aboutSection?.SectionData;
            var stats = statsSection?.SectionData;
            var catalog = catalogSection?.SectionData;
            var whyChooseUs = whyChooseUsSection?.SectionData;
            var brandSlider = brandSliderSection?.SectionData;
            var features = featuresSection?.SectionData;
            var callToAction = callToActionSection?.SectionData;
            var location = locationSection?.SectionData;
            var newsletter = newsletterSection?.SectionData;

            return Results.Ok(new HomeDataResponse(
                hero.HasValue && hero.Value.ValueKind == JsonValueKind.Object ? hero.Value : BuildDefaultHeroData(),
                about.HasValue && about.Value.ValueKind == JsonValueKind.Object ? about.Value : BuildDefaultAboutData(),
                stats.HasValue && stats.Value.ValueKind == JsonValueKind.Object ? stats.Value : BuildDefaultStatsData(),
                catalog.HasValue && catalog.Value.ValueKind == JsonValueKind.Object ? catalog.Value : BuildDefaultCatalogData(),
                whyChooseUs.HasValue && whyChooseUs.Value.ValueKind == JsonValueKind.Object ? whyChooseUs.Value : BuildDefaultWhyChooseUsData(),
                brandSlider.HasValue && brandSlider.Value.ValueKind == JsonValueKind.Object ? brandSlider.Value : BuildDefaultBrandSliderData(),
                features.HasValue && features.Value.ValueKind == JsonValueKind.Object ? features.Value : BuildDefaultFeaturesData(),
                callToAction.HasValue && callToAction.Value.ValueKind == JsonValueKind.Object ? callToAction.Value : BuildDefaultCallToActionData(),
                location.HasValue && location.Value.ValueKind == JsonValueKind.Object ? location.Value : BuildDefaultLocationData(),
                newsletter.HasValue && newsletter.Value.ValueKind == JsonValueKind.Object ? newsletter.Value : BuildDefaultNewsletterData(),
                slider,
                navigation is null ? null : ToNavigationResponse(navigation),
                footer is null ? null : ToNavigationResponse(footer),
                menus));
        });

        return group;
    }

    private static NavigationConfigResponse ToNavigationResponse(NavigationConfigItem item)
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

    private static string ToWidthVariantString(Domain.NavigationEngine.NavWidthVariant widthVariant)
    {
        return widthVariant switch
        {
            Domain.NavigationEngine.NavWidthVariant.Full => "full",
            Domain.NavigationEngine.NavWidthVariant.Boxed => "boxed",
            _ => "container",
        };
    }

    private static JsonElement BuildDefaultHeroData()
    {
        using var document = JsonDocument.Parse("""{"title":"","subtitle":"","primaryCtaLabel":"","primaryCtaHref":""}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultAboutData()
    {
        using var document = JsonDocument.Parse("""{"title":"","subtitle":"","content":[],"image":{"src":"","alt":""}}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultStatsData()
    {
        using var document = JsonDocument.Parse("""{"backgroundToken":"background","borderToken":"border","stats":[]}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultCatalogData()
    {
        using var document = JsonDocument.Parse("""{"heading":"","subheading":"","categories":[]}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultWhyChooseUsData()
    {
        using var document = JsonDocument.Parse("""{"heading":"","subheading":"","items":[]}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultBrandSliderData()
    {
        using var document = JsonDocument.Parse("""{"heading":"","pauseOnHover":true,"animationDuration":40,"logos":[]}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultFeaturesData()
    {
        using var document = JsonDocument.Parse("""{"title":"","description":"","imageSrc":"","imageAlt":"","bullets":[]}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultCallToActionData()
    {
        using var document = JsonDocument.Parse("""{"title":"","description":"","buttonText":"","buttonHref":"","label":"","href":""}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultLocationData()
    {
        using var document = JsonDocument.Parse("""{"displayName":"","title":"","address":"","buttonText":"","buttonHref":"","imageSrc":"","imageAlt":"","imageClassName":"","mapEmbedUrl":"","mapTitle":"","placeId":"","latitude":0,"longitude":0,"timings":[],"contact":{"phone":"","email":""}}""");
        return document.RootElement.Clone();
    }

    private static JsonElement BuildDefaultNewsletterData()
    {
        using var document = JsonDocument.Parse("""{"title":"","description":"","placeholderName":"","placeholderEmail":"","buttonText":"","trustNote":"","imageSrc":"","imageAlt":"","image":""}""");
        return document.RootElement.Clone();
    }

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

    public sealed record HomeDataResponse(
        JsonElement Hero,
        JsonElement About,
        JsonElement Stats,
        JsonElement Catalog,
        JsonElement WhyChooseUs,
        JsonElement BrandSlider,
        JsonElement Features,
        JsonElement CallToAction,
        JsonElement Location,
        JsonElement Newsletter,
        SliderConfigDto Slider,
        NavigationConfigResponse? Navigation,
        NavigationConfigResponse? Footer,
        IReadOnlyList<MenuRenderGroupItem> Menus);
}
