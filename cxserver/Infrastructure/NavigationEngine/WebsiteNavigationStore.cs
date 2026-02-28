using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.NavigationEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.NavigationEngine;

internal sealed class WebsiteNavigationStore : IWebsiteNavigationStore
{
    private static readonly JsonDocument DefaultNavigationLayout = JsonDocument.Parse("""
        {"variant":"container","zoneOrder":["left","center","right"],"menuAlign":"center","logoPosition":"left","menuSize":"medium"}
        """);
    private static readonly JsonDocument DefaultNavigationStyle = JsonDocument.Parse("""
        {"backgroundToken":"header-bg","textToken":"header-foreground","hoverToken":"menu-hover","activeToken":"primary","dropdownToken":"card","borderToken":"border","scrollBackgroundToken":"header-bg","scrollTextToken":"foreground"}
        """);
    private static readonly JsonDocument DefaultNavigationBehavior = JsonDocument.Parse("""
        {"sticky":true,"scrollShadow":true,"transparentOnTop":false,"blur":true,"borderBottom":true,"mobileOverlay":true}
        """);
    private static readonly JsonDocument DefaultNavigationComponent = JsonDocument.Parse("""
        {"left":["logo"],"center":["menu"],"right":["themeSwitch","auth"],"logo":{"type":"text","text":"CodexSun","showText":true,"textPosition":"right","size":"medium"},"auth":{"enabled":true,"loginPath":"/auth/login","dashboardPath":"/app"},"cta":{"enabled":false,"label":"","url":"","target":"_self"}}
        """);

    private static readonly JsonDocument DefaultFooterLayout = JsonDocument.Parse("""
        {"variant":"container","columns":4,"sectionOrder":["about","links","legal","social","businessHours","newsletter","payments","bottom"]}
        """);
    private static readonly JsonDocument DefaultFooterStyle = JsonDocument.Parse("""
        {"backgroundToken":"footer-bg","textToken":"footer-foreground","linkToken":"link","linkHoverToken":"link-hover","borderTop":true,"spacing":"normal","columnGap":"normal"}
        """);
    private static readonly JsonDocument DefaultFooterBehavior = JsonDocument.Parse("""
        {"showDynamicYear":true,"showNewsletter":false,"showPayments":false}
        """);
    private static readonly JsonDocument DefaultFooterComponent = JsonDocument.Parse("""
        {"about":{"enabled":true,"title":"About","content":"Dynamic tenant footer."},"links":{"enabled":true,"menuGroupSlug":"footer"},"legal":{"enabled":true,"items":[{"label":"Privacy","url":"/privacy-policy"},{"label":"Terms","url":"/terms"},{"label":"Support","url":"/support"}]},"social":{"enabled":false,"items":[]},"newsletter":{"enabled":false,"title":"Newsletter","description":"Get updates."},"businessHours":{"enabled":false,"items":[]},"payments":{"enabled":false,"providers":[]},"bottom":{"enabled":true,"copyright":"All rights reserved","developedBy":{"enabled":false,"label":"","url":""}}}
        """);

    private readonly ITenantDbContextAccessor _tenantDbContextAccessor;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public WebsiteNavigationStore(
        ITenantDbContextAccessor tenantDbContextAccessor,
        ITenantContext tenantContext,
        IDateTimeProvider dateTimeProvider)
    {
        _tenantDbContextAccessor = tenantDbContextAccessor;
        _tenantContext = tenantContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<NavigationConfigItem?> GetWebNavigationConfigAsync(Guid? tenantId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scopedTenantId = ResolveTenantScope(tenantId);
        var config = await ResolveNavigationAsync(dbContext, scopedTenantId, cancellationToken);
        if (config is not null)
        {
            return Map(config);
        }

        return new NavigationConfigItem(
            Guid.Empty,
            null,
            NavWidthVariant.Container,
            CloneJson(DefaultNavigationLayout),
            CloneJson(DefaultNavigationStyle),
            CloneJson(DefaultNavigationBehavior),
            CloneJson(DefaultNavigationComponent),
            true,
            DateTimeOffset.MinValue,
            DateTimeOffset.MinValue);
    }

    public async Task<NavigationConfigItem> UpsertWebNavigationConfigAsync(UpsertNavigationConfigInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scopedTenantId = ResolveTenantScope(input.TenantId);
        var entity = await dbContext.WebNavigationConfigs
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == scopedTenantId, cancellationToken);

        if (entity is null)
        {
            var widthVariant = ParseNavigationWidthVariant(input.LayoutConfig, strict: true);
            entity = WebNavigationConfig.Create(
                Guid.NewGuid(),
                scopedTenantId,
                widthVariant,
                CloneJson(input.LayoutConfig),
                CloneJson(input.StyleConfig),
                CloneJson(input.BehaviorConfig),
                CloneJson(input.ComponentConfig),
                input.IsActive,
                _dateTimeProvider.UtcNow);

            await dbContext.WebNavigationConfigs.AddAsync(entity, cancellationToken);
        }
        else
        {
            var widthVariant = ParseNavigationWidthVariant(input.LayoutConfig, strict: true);
            entity.Update(
                widthVariant,
                CloneJson(input.LayoutConfig),
                CloneJson(input.StyleConfig),
                CloneJson(input.BehaviorConfig),
                CloneJson(input.ComponentConfig),
                input.IsActive,
                _dateTimeProvider.UtcNow);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<NavigationConfigItem> ResetWebNavigationConfigAsync(Guid? tenantId, CancellationToken cancellationToken)
    {
        return await UpsertWebNavigationConfigAsync(
            new UpsertNavigationConfigInput(
                tenantId,
                CloneJson(DefaultNavigationLayout),
                CloneJson(DefaultNavigationStyle),
                CloneJson(DefaultNavigationBehavior),
                CloneJson(DefaultNavigationComponent),
                true),
            cancellationToken);
    }

    public async Task<NavigationConfigItem?> GetFooterConfigAsync(Guid? tenantId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scopedTenantId = ResolveTenantScope(tenantId);
        var config = await ResolveFooterAsync(dbContext, scopedTenantId, cancellationToken);
        return config is null ? null : Map(config);
    }

    public async Task<NavigationConfigItem> UpsertFooterConfigAsync(UpsertNavigationConfigInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var scopedTenantId = ResolveTenantScope(input.TenantId);
        var entity = await dbContext.FooterConfigs
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == scopedTenantId, cancellationToken);

        if (entity is null)
        {
            entity = FooterConfig.Create(
                Guid.NewGuid(),
                scopedTenantId,
                CloneJson(input.LayoutConfig),
                CloneJson(input.StyleConfig),
                CloneJson(input.BehaviorConfig),
                CloneJson(input.ComponentConfig),
                input.IsActive,
                _dateTimeProvider.UtcNow);

            await dbContext.FooterConfigs.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.Update(
                CloneJson(input.LayoutConfig),
                CloneJson(input.StyleConfig),
                CloneJson(input.BehaviorConfig),
                CloneJson(input.ComponentConfig),
                input.IsActive,
                _dateTimeProvider.UtcNow);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<NavigationConfigItem> ResetFooterConfigAsync(Guid? tenantId, CancellationToken cancellationToken)
    {
        return await UpsertFooterConfigAsync(
            new UpsertNavigationConfigInput(
                tenantId,
                CloneJson(DefaultFooterLayout),
                CloneJson(DefaultFooterStyle),
                CloneJson(DefaultFooterBehavior),
                CloneJson(DefaultFooterComponent),
                true),
            cancellationToken);
    }

    private async Task<WebNavigationConfig?> ResolveNavigationAsync(TenantDbContext dbContext, Guid? tenantId, CancellationToken cancellationToken)
    {
        if (tenantId.HasValue)
        {
            var tenantConfig = await dbContext.WebNavigationConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

            if (tenantConfig is not null)
            {
                return tenantConfig;
            }
        }

        return await dbContext.WebNavigationConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == null, cancellationToken);
    }

    private async Task<FooterConfig?> ResolveFooterAsync(TenantDbContext dbContext, Guid? tenantId, CancellationToken cancellationToken)
    {
        if (tenantId.HasValue)
        {
            var tenantConfig = await dbContext.FooterConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

            if (tenantConfig is not null)
            {
                return tenantConfig;
            }
        }

        return await dbContext.FooterConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == null, cancellationToken);
    }

    private Guid? ResolveTenantScope(Guid? requestedTenantId)
    {
        if (!requestedTenantId.HasValue)
        {
            return null;
        }

        if (!_tenantContext.TenantId.HasValue)
        {
            throw new InvalidOperationException("Tenant scope is not available in current context.");
        }

        if (requestedTenantId != _tenantContext.TenantId)
        {
            throw new InvalidOperationException("Cannot access another tenant scope.");
        }

        return requestedTenantId;
    }

    private static JsonDocument CloneJson(JsonDocument source)
    {
        return JsonDocument.Parse(source.RootElement.GetRawText());
    }

    private static NavigationConfigItem Map(WebNavigationConfig entity)
    {
        var widthVariant = entity.WidthVariant;
        var parsedVariant = ParseNavigationWidthVariant(entity.LayoutConfig, strict: false);
        if (parsedVariant != widthVariant)
        {
            widthVariant = parsedVariant;
        }

        return new NavigationConfigItem(
            entity.Id,
            entity.TenantId,
            widthVariant,
            CloneJson(entity.LayoutConfig),
            CloneJson(entity.StyleConfig),
            CloneJson(entity.BehaviorConfig),
            CloneJson(entity.ComponentConfig),
            entity.IsActive,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc);
    }

    private static NavigationConfigItem Map(FooterConfig entity)
    {
        return new NavigationConfigItem(
            entity.Id,
            entity.TenantId,
            NavWidthVariant.Container,
            CloneJson(entity.LayoutConfig),
            CloneJson(entity.StyleConfig),
            CloneJson(entity.BehaviorConfig),
            CloneJson(entity.ComponentConfig),
            entity.IsActive,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc);
    }

    private static NavWidthVariant ParseNavigationWidthVariant(JsonDocument layoutConfig, bool strict)
    {
        if (!layoutConfig.RootElement.TryGetProperty("variant", out var variantElement))
        {
            return NavWidthVariant.Container;
        }

        if (variantElement.ValueKind is not JsonValueKind.String)
        {
            if (strict)
            {
                throw new ArgumentException("layoutConfig.variant must be a string.");
            }

            return NavWidthVariant.Container;
        }

        var raw = variantElement.GetString()?.Trim().ToLowerInvariant();
        return raw switch
        {
            "container" => NavWidthVariant.Container,
            "full" => NavWidthVariant.Full,
            "boxed" => NavWidthVariant.Boxed,
            _ when strict => throw new ArgumentException("layoutConfig.variant must be one of: container, full, boxed."),
            _ => NavWidthVariant.Container,
        };
    }
}
