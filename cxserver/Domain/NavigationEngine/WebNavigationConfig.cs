using System.Text.Json;
using cxserver.Domain.Common;

namespace cxserver.Domain.NavigationEngine;

public sealed class WebNavigationConfig : AggregateRoot, ISoftDeletable
{
    private WebNavigationConfig(
        Guid id,
        Guid? tenantId,
        NavWidthVariant widthVariant,
        JsonDocument layoutConfig,
        JsonDocument styleConfig,
        JsonDocument behaviorConfig,
        JsonDocument componentConfig,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        TenantId = tenantId;
        WidthVariant = widthVariant;
        LayoutConfig = layoutConfig;
        StyleConfig = styleConfig;
        BehaviorConfig = behaviorConfig;
        ComponentConfig = componentConfig;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private WebNavigationConfig() : base(Guid.NewGuid())
    {
        WidthVariant = NavWidthVariant.Container;
        LayoutConfig = JsonDocument.Parse("{}");
        StyleConfig = JsonDocument.Parse("{}");
        BehaviorConfig = JsonDocument.Parse("{}");
        ComponentConfig = JsonDocument.Parse("{}");
    }

    public Guid? TenantId { get; private set; }
    public NavWidthVariant WidthVariant { get; private set; }
    public JsonDocument LayoutConfig { get; private set; }
    public JsonDocument StyleConfig { get; private set; }
    public JsonDocument BehaviorConfig { get; private set; }
    public JsonDocument ComponentConfig { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public static WebNavigationConfig Create(
        Guid id,
        Guid? tenantId,
        NavWidthVariant widthVariant,
        JsonDocument layoutConfig,
        JsonDocument styleConfig,
        JsonDocument behaviorConfig,
        JsonDocument componentConfig,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        ArgumentNullException.ThrowIfNull(layoutConfig);
        ArgumentNullException.ThrowIfNull(styleConfig);
        ArgumentNullException.ThrowIfNull(behaviorConfig);
        ArgumentNullException.ThrowIfNull(componentConfig);

        return new WebNavigationConfig(
            id,
            tenantId,
            widthVariant,
            layoutConfig,
            styleConfig,
            behaviorConfig,
            componentConfig,
            isActive,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Update(
        NavWidthVariant widthVariant,
        JsonDocument layoutConfig,
        JsonDocument styleConfig,
        JsonDocument behaviorConfig,
        JsonDocument componentConfig,
        bool isActive,
        DateTimeOffset nowUtc)
    {
        ArgumentNullException.ThrowIfNull(layoutConfig);
        ArgumentNullException.ThrowIfNull(styleConfig);
        ArgumentNullException.ThrowIfNull(behaviorConfig);
        ArgumentNullException.ThrowIfNull(componentConfig);

        WidthVariant = widthVariant;
        LayoutConfig = layoutConfig;
        StyleConfig = styleConfig;
        BehaviorConfig = behaviorConfig;
        ComponentConfig = componentConfig;
        IsActive = isActive;
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
}
