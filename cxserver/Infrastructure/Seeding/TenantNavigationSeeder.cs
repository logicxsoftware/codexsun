using System.Text.Json;
using cxserver.Domain.NavigationEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantNavigationSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantNavigationSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;

        var navigation = await dbContext.WebNavigationConfigs
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == null, cancellationToken);

        if (navigation is null)
        {
            navigation = WebNavigationConfig.Create(
                Guid.NewGuid(),
                null,
                JsonDocument.Parse("""{"variant":"container","zoneOrder":["left","center","right"],"menuAlign":"center","logoPosition":"left","menuSize":"medium"}"""),
                JsonDocument.Parse("""{"backgroundToken":"header-bg","textToken":"header-foreground","hoverToken":"menu-hover","activeToken":"primary","dropdownToken":"card","borderToken":"border","scrollBackgroundToken":"header-bg","scrollTextToken":"header-foreground"}"""),
                JsonDocument.Parse("""{"sticky":true,"scrollShadow":true,"transparentOnTop":true,"blur":true,"borderBottom":true,"mobileOverlay":true}"""),
                JsonDocument.Parse("""{"left":["logo"],"center":["menu"],"right":["themeSwitch","auth"],"logo":{"type":"text","text":"Codexsun","showText":true,"textPosition":"right","size":"medium"},"auth":{"enabled":true,"loginPath":"/login","dashboardPath":"/app"},"cta":{"enabled":true,"label":"Get Started","url":"/signup","target":"_self"}}"""),
                true,
                now);

            await dbContext.WebNavigationConfigs.AddAsync(navigation, cancellationToken);
        }
        else
        {
            navigation.Update(
                JsonDocument.Parse("""{"variant":"container","zoneOrder":["left","center","right"],"menuAlign":"center","logoPosition":"left","menuSize":"medium"}"""),
                JsonDocument.Parse("""{"backgroundToken":"header-bg","textToken":"header-foreground","hoverToken":"menu-hover","activeToken":"primary","dropdownToken":"card","borderToken":"border","scrollBackgroundToken":"header-bg","scrollTextToken":"header-foreground"}"""),
                JsonDocument.Parse("""{"sticky":true,"scrollShadow":true,"transparentOnTop":true,"blur":true,"borderBottom":true,"mobileOverlay":true}"""),
                JsonDocument.Parse("""{"left":["logo"],"center":["menu"],"right":["themeSwitch","auth"],"logo":{"type":"text","text":"Codexsun","showText":true,"textPosition":"right","size":"medium"},"auth":{"enabled":true,"loginPath":"/login","dashboardPath":"/app"},"cta":{"enabled":true,"label":"Get Started","url":"/signup","target":"_self"}}"""),
                true,
                now);
        }

        var footer = await dbContext.FooterConfigs
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == null, cancellationToken);

        if (footer is null)
        {
            footer = FooterConfig.Create(
                Guid.NewGuid(),
                null,
                JsonDocument.Parse("""{"variant":"container","columns":4,"sectionOrder":["about","links","legal","social","businessHours","newsletter","payments","bottom"]}"""),
                JsonDocument.Parse("""{"backgroundToken":"footer-bg","textToken":"foreground","linkToken":"link","linkHoverToken":"primary","borderTop":true,"spacing":"normal","columnGap":"normal"}"""),
                JsonDocument.Parse("""{"showDynamicYear":true,"showNewsletter":true,"showPayments":true}"""),
                JsonDocument.Parse("""{"about":{"enabled":true,"title":"Codexsun","content":"Codexsun is a modern SaaS software company delivering scalable digital products for startups, enterprises, and growing businesses. We build intelligent systems that power productivity, automation, and digital transformation."},"links":{"enabled":true,"menuGroupSlug":"footer"},"legal":{"enabled":true,"items":[{"label":"Privacy Policy","url":"/privacy-policy","target":"_self"},{"label":"Terms","url":"/terms","target":"_self"},{"label":"Cookie Policy","url":"/cookies","target":"_self"},{"label":"Refund Policy","url":"/refund-policy","target":"_self"}]},"social":{"enabled":true,"items":[{"icon":"linkedin","label":"LinkedIn","url":"https://linkedin.com/company/codexsun","target":"_blank"},{"icon":"twitter","label":"Twitter","url":"https://twitter.com/codexsun","target":"_blank"},{"icon":"github","label":"GitHub","url":"https://github.com/codexsun","target":"_blank"},{"icon":"facebook","label":"Facebook","url":"https://facebook.com/codexsun","target":"_blank"},{"icon":"instagram","label":"Instagram","url":"https://instagram.com/codexsun","target":"_blank"}]},"newsletter":{"enabled":true,"title":"Stay Updated","description":"Subscribe to receive product updates and company news."},"businessHours":{"enabled":true,"items":[{"day":"Monday - Friday","hours":"9:00 AM - 6:00 PM"},{"day":"Saturday","hours":"10:00 AM - 4:00 PM"},{"day":"Sunday","hours":"Closed"}]},"payments":{"enabled":true,"providers":["Stripe","Razorpay","PayPal","Visa","Mastercard"]},"bottom":{"enabled":true,"copyright":"© {YEAR} Codexsun. All rights reserved.","developedBy":{"enabled":true,"label":"Codexsun Engineering Team","url":"https://codexsun.com"}}}"""),
                true,
                now);

            await dbContext.FooterConfigs.AddAsync(footer, cancellationToken);
        }
        else
        {
            footer.Update(
                JsonDocument.Parse("""{"variant":"container","columns":4,"sectionOrder":["about","links","legal","social","businessHours","newsletter","payments","bottom"]}"""),
                JsonDocument.Parse("""{"backgroundToken":"footer-bg","textToken":"foreground","linkToken":"link","linkHoverToken":"primary","borderTop":true,"spacing":"normal","columnGap":"normal"}"""),
                JsonDocument.Parse("""{"showDynamicYear":true,"showNewsletter":true,"showPayments":true}"""),
                JsonDocument.Parse("""{"about":{"enabled":true,"title":"Codexsun","content":"Codexsun is a modern SaaS software company delivering scalable digital products for startups, enterprises, and growing businesses. We build intelligent systems that power productivity, automation, and digital transformation."},"links":{"enabled":true,"menuGroupSlug":"footer"},"legal":{"enabled":true,"items":[{"label":"Privacy Policy","url":"/privacy-policy","target":"_self"},{"label":"Terms","url":"/terms","target":"_self"},{"label":"Cookie Policy","url":"/cookies","target":"_self"},{"label":"Refund Policy","url":"/refund-policy","target":"_self"}]},"social":{"enabled":true,"items":[{"icon":"linkedin","label":"LinkedIn","url":"https://linkedin.com/company/codexsun","target":"_blank"},{"icon":"twitter","label":"Twitter","url":"https://twitter.com/codexsun","target":"_blank"},{"icon":"github","label":"GitHub","url":"https://github.com/codexsun","target":"_blank"},{"icon":"facebook","label":"Facebook","url":"https://facebook.com/codexsun","target":"_blank"},{"icon":"instagram","label":"Instagram","url":"https://instagram.com/codexsun","target":"_blank"}]},"newsletter":{"enabled":true,"title":"Stay Updated","description":"Subscribe to receive product updates and company news."},"businessHours":{"enabled":true,"items":[{"day":"Monday - Friday","hours":"9:00 AM - 6:00 PM"},{"day":"Saturday","hours":"10:00 AM - 4:00 PM"},{"day":"Sunday","hours":"Closed"}]},"payments":{"enabled":true,"providers":["Stripe","Razorpay","PayPal","Visa","Mastercard"]},"bottom":{"enabled":true,"copyright":"© {YEAR} Codexsun. All rights reserved.","developedBy":{"enabled":true,"label":"Codexsun Engineering Team","url":"https://codexsun.com"}}}"""),
                true,
                now);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
