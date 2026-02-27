using System.Text.Json;
using cxserver.Domain.WebEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantWebsitePageSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantWebsitePageSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        var definitions = BuildDefinitions();
        var now = _dateTimeProvider.UtcNow;

        foreach (var definition in definitions)
        {
            var page = await dbContext.WebsitePages
                .Include(x => x.Sections)
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Slug == definition.Slug, cancellationToken);

            if (page is null)
            {
                page = Page.Create(
                    Guid.NewGuid(),
                    definition.Slug,
                    definition.Title,
                    definition.SeoTitle,
                    definition.SeoDescription,
                    now);

                for (var i = 0; i < definition.Sections.Count; i++)
                {
                    var section = definition.Sections[i];
                    page.AddSection(
                        Guid.NewGuid(),
                        section.SectionType,
                        i,
                        JsonDocument.Parse(section.SectionDataJson),
                        true,
                        now);
                }

                page.Publish(now);
                await dbContext.WebsitePages.AddAsync(page, cancellationToken);
                continue;
            }

            if (!page.IsPublished)
            {
                page.Publish(now);
            }

            if (page.Sections.Count == 0)
            {
                for (var i = 0; i < definition.Sections.Count; i++)
                {
                    var section = definition.Sections[i];
                    page.AddSection(
                        Guid.NewGuid(),
                        section.SectionType,
                        i,
                        JsonDocument.Parse(section.SectionDataJson),
                        true,
                        now);
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyList<PageSeedDefinition> BuildDefinitions()
    {
        return
        [
            new PageSeedDefinition(
                "home",
                "Home",
                "Home",
                "Home page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Welcome\",\"subtitle\":\"Dynamic multi-tenant website engine\",\"primaryCtaLabel\":\"Explore Services\",\"primaryCtaHref\":\"/services\"}"),
                    new SectionSeedDefinition(SectionType.Features, "{\"items\":[{\"title\":\"Tenant Isolated\",\"description\":\"Content is isolated per tenant database.\"},{\"title\":\"Section Driven\",\"description\":\"Layouts are rendered from ordered section data.\"},{\"title\":\"Extensible\",\"description\":\"New section types can be added safely.\"}]}"),
                    new SectionSeedDefinition(SectionType.CallToAction, "{\"title\":\"Start your project\",\"label\":\"Contact Us\",\"href\":\"/contact\"}"),
                    new SectionSeedDefinition(SectionType.Footer, "{\"columns\":[{\"title\":\"Company\",\"links\":[{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]},{\"title\":\"Resources\",\"links\":[{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Services\",\"href\":\"/services\"}]}]}")]
            ),
            new PageSeedDefinition(
                "about",
                "About",
                "About",
                "About page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"About Us\",\"subtitle\":\"We build reliable SaaS platforms.\"}"),
                    new SectionSeedDefinition(SectionType.WhyChooseUs, "{\"items\":[{\"title\":\"Architecture\",\"description\":\"Clean architecture with clear boundaries.\"},{\"title\":\"Delivery\",\"description\":\"Vertical-slice implementation for speed.\"},{\"title\":\"Scale\",\"description\":\"Provider-agnostic persistence strategy.\"}]}"),
                    new SectionSeedDefinition(SectionType.Stats, "{\"items\":[{\"label\":\"Tenants\",\"value\":\"100+\"},{\"label\":\"Uptime\",\"value\":\"99.9%\"},{\"label\":\"Deployments\",\"value\":\"500+\"}]}"),
                    new SectionSeedDefinition(SectionType.Footer, "{\"columns\":[{\"title\":\"Navigate\",\"links\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"Services\",\"href\":\"/services\"}]},{\"title\":\"Connect\",\"links\":[{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}]}")]
            ),
            new PageSeedDefinition(
                "services",
                "Services",
                "Services",
                "Services page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Our Services\",\"subtitle\":\"Engineering and delivery offerings.\"}"),
                    new SectionSeedDefinition(SectionType.ProductRange, "{\"products\":[{\"name\":\"Platform Engineering\",\"description\":\"Cloud-native backend and DevOps enablement.\"},{\"name\":\"Frontend Engineering\",\"description\":\"Type-safe React architecture and design systems.\"},{\"name\":\"Data Services\",\"description\":\"Schema design, migrations, and performance tuning.\"}]}"),
                    new SectionSeedDefinition(SectionType.Testimonial, "{\"items\":[{\"author\":\"CTO\",\"quote\":\"Delivery quality and speed exceeded expectations.\"},{\"author\":\"Product Lead\",\"quote\":\"The platform is stable and easy to evolve.\"}]}"),
                    new SectionSeedDefinition(SectionType.Footer, "{\"columns\":[{\"title\":\"Services\",\"links\":[{\"label\":\"Platform Engineering\",\"href\":\"/services\"},{\"label\":\"Frontend Engineering\",\"href\":\"/services\"}]},{\"title\":\"Company\",\"links\":[{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}]}")]
            ),
            new PageSeedDefinition(
                "blog",
                "Blog",
                "Blog",
                "Blog page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Blog\",\"subtitle\":\"Insights, architecture, and release notes.\"}"),
                    new SectionSeedDefinition(SectionType.BlogShow, "{\"limit\":6,\"title\":\"Latest Posts\"}"),
                    new SectionSeedDefinition(SectionType.Newsletter, "{\"title\":\"Subscribe\",\"subtitle\":\"Get platform updates in your inbox.\",\"placeholder\":\"Email\",\"buttonLabel\":\"Subscribe\"}"),
                    new SectionSeedDefinition(SectionType.Footer, "{\"columns\":[{\"title\":\"Read\",\"links\":[{\"label\":\"Latest Posts\",\"href\":\"/blog\"},{\"label\":\"About\",\"href\":\"/about\"}]},{\"title\":\"Reach\",\"links\":[{\"label\":\"Contact\",\"href\":\"/contact\"},{\"label\":\"Services\",\"href\":\"/services\"}]}]}")]
            ),
            new PageSeedDefinition(
                "contact",
                "Contact",
                "Contact",
                "Contact page",
                [
                    new SectionSeedDefinition(SectionType.Menu, "{\"items\":[{\"label\":\"Home\",\"href\":\"/\"},{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"},{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Contact\",\"href\":\"/contact\"}]}"),
                    new SectionSeedDefinition(SectionType.Hero, "{\"title\":\"Contact Us\",\"subtitle\":\"Talk to our team about your product and platform goals.\"}"),
                    new SectionSeedDefinition(SectionType.CallToAction, "{\"title\":\"Schedule a consultation\",\"label\":\"Email Team\",\"href\":\"mailto:team@example.com\"}"),
                    new SectionSeedDefinition(SectionType.BrandSlider, "{\"brands\":[{\"name\":\"Acme\"},{\"name\":\"Contoso\"},{\"name\":\"Globex\"},{\"name\":\"Initech\"}]}"),
                    new SectionSeedDefinition(SectionType.Footer, "{\"columns\":[{\"title\":\"Company\",\"links\":[{\"label\":\"About\",\"href\":\"/about\"},{\"label\":\"Services\",\"href\":\"/services\"}]},{\"title\":\"Support\",\"links\":[{\"label\":\"Blog\",\"href\":\"/blog\"},{\"label\":\"Home\",\"href\":\"/\"}]}]}")]
            )
        ];
    }

    private sealed record PageSeedDefinition(
        string Slug,
        string Title,
        string SeoTitle,
        string SeoDescription,
        IReadOnlyList<SectionSeedDefinition> Sections);

    private sealed record SectionSeedDefinition(
        SectionType SectionType,
        string SectionDataJson);
}
