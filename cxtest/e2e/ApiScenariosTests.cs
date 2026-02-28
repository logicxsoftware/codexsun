using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cxtest.TestKit;
using MySqlConnector;

namespace cxtest.e2e;

public sealed class ApiScenariosTests
{
    [Fact]
    public async Task Tenant_And_Web_Endpoints_Handle_Success_And_Failure_Paths()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_api_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboard = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "api-tenant",
            Domain = "api-tenant.localhost",
            Name = "Api Tenant",
            DatabaseName = "tenant_api_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}"
        });

        Assert.Equal(HttpStatusCode.OK, onboard.StatusCode);

        client.DefaultRequestHeaders.Host = "api-tenant.localhost";
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/tenant/current")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/web/menus")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/slider")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/web/theme")).StatusCode);

        var badCreate = await client.PostAsJsonAsync("/api/admin/menu-groups", new
        {
            TenantId = Guid.NewGuid(),
            Type = 0,
            Name = "",
            Slug = "",
            Description = (string?)null,
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.InternalServerError, badCreate.StatusCode);

        client.DefaultRequestHeaders.Host = "unknown.localhost";
        var unknownTenant = await client.GetAsync("/api/web/menus");
        Assert.Equal(HttpStatusCode.NotFound, unknownTenant.StatusCode);

        client.DefaultRequestHeaders.Host = "api-tenant.localhost";
        var notFound = await client.PatchAsJsonAsync($"/api/slider/slides/{Guid.NewGuid()}", new
        {
            Order = 0,
            Title = "x",
            Tagline = "y",
            ActionText = "z",
            ActionHref = "/",
            CtaColor = 0,
            Duration = 4000,
            Direction = 0,
            Variant = 0,
            Intensity = 0,
            BackgroundMode = 0,
            ShowOverlay = true,
            OverlayToken = "muted/70",
            BackgroundUrl = "https://images.unsplash.com/photo-1?w=1920",
            MediaType = 0,
            YoutubeVideoId = (string?)null,
            IsActive = true,
            Highlights = Array.Empty<object>()
        });

        Assert.Equal(HttpStatusCode.NotFound, notFound.StatusCode);
    }
}
