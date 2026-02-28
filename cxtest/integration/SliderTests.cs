using cxserver.Application.Abstractions;
using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using cxserver.Domain.SliderEngine;
using cxtest.TestKit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace cxtest.integration;

public sealed class SliderTests
{
    [Fact]
    public async Task Slider_Config_And_Slide_CRUD_Works_With_Validation()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_slider_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("slider-test", "slider-test.localhost", "Slider Test", "tenant_slider_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_slider_db", StringComparison.OrdinalIgnoreCase);
        provider.GetRequiredService<ITenantContextAccessor>().SetTenant(new TenantSession(tenant.TenantId, tenant.Name, "slider-test.localhost", tenantConnection));

        var store = provider.GetRequiredService<ISliderStore>();
        var config = await store.GetOrCreateAsync(tenant.TenantId, CancellationToken.None);
        Assert.NotNull(config);

        var created = await store.CreateSlideAsync(
            tenant.TenantId,
            new CreateSlideInput(
                10,
                "Title",
                "Tagline",
                "Start",
                "/signup",
                SliderCtaColor.Primary,
                4000,
                SliderDirection.Left,
                SliderVariant.Saas,
                SliderIntensity.Medium,
                SliderBackgroundMode.Cinematic,
                true,
                "muted/70",
                "https://images.unsplash.com/photo-1?w=1920",
                SliderMediaType.Image,
                null,
                true,
                [new SlideHighlightInput("One", "primary", 0)]),
            CancellationToken.None);

        Assert.Equal(SliderCtaColor.Primary, created.CtaColor);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            store.CreateSlideAsync(
                tenant.TenantId,
                new CreateSlideInput(
                    11,
                    "Bad",
                    "Bad",
                    "Bad",
                    "javascript:alert(1)",
                    SliderCtaColor.Primary,
                    4000,
                    SliderDirection.Left,
                    SliderVariant.Saas,
                    SliderIntensity.Medium,
                    SliderBackgroundMode.Cinematic,
                    true,
                    "muted/70",
                    "https://images.unsplash.com/photo-2?w=1920",
                    SliderMediaType.Image,
                    null,
                    true,
                    []),
                CancellationToken.None));
    }

    [Fact]
    public async Task Layer_And_Highlight_Relations_Remain_Consistent()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_slider_rel_db");

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<ISender>();
        var tenant = await sender.Send(new OnboardTenantCommand("slider-rel", "slider-rel.localhost", "Slider Rel", "tenant_slider_rel_db", "{}", "{}"));
        var tenantConnection = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant_slider_rel_db", StringComparison.OrdinalIgnoreCase);
        provider.GetRequiredService<ITenantContextAccessor>().SetTenant(new TenantSession(tenant.TenantId, tenant.Name, "slider-rel.localhost", tenantConnection));
        var store = provider.GetRequiredService<ISliderStore>();

        var config = await store.GetOrCreateAsync(tenant.TenantId, CancellationToken.None);
        var target = config.Slides.OrderBy(x => x.Order).FirstOrDefault();
        if (target is null)
        {
            target = await store.CreateSlideAsync(
                tenant.TenantId,
                new CreateSlideInput(
                    0,
                    "Base",
                    "Base",
                    "Go",
                    "/",
                    SliderCtaColor.Primary,
                    4000,
                    SliderDirection.Left,
                    SliderVariant.Saas,
                    SliderIntensity.Medium,
                    SliderBackgroundMode.Normal,
                    true,
                    "muted/70",
                    "https://images.unsplash.com/photo-3?w=1920",
                    SliderMediaType.Image,
                    null,
                    true,
                    []),
                CancellationToken.None);
        }

        var layer = await store.CreateLayerAsync(
            tenant.TenantId,
            new CreateLayerInput(target.Id, 99, SliderLayerType.Text, "Layer", null, 50, 50, "200px", SliderLayerAnimationFrom.Fade, 100, 500, "ease-out", "all"),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, layer.Id);
        var deleted = await store.DeleteLayerAsync(tenant.TenantId, layer.Id, CancellationToken.None);
        Assert.True(deleted);
    }
}
