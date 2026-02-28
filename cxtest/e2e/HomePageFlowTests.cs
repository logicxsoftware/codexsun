using System.Diagnostics;
using System.Net;
using cxtest.TestKit;
using MySqlConnector;

namespace cxtest.e2e;

public sealed class HomePageFlowTests
{
    [Fact]
    public async Task Home_Page_Data_Flow_Is_Stable_And_Performant()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();
        client.DefaultRequestHeaders.Host = "localhost";

        var sw = Stopwatch.StartNew();
        var tenantTask = client.GetAsync("/api/tenant/current");
        var menusTask = client.GetAsync("/api/web/menus");
        var navTask = client.GetAsync("/api/web/navigation-config");
        var footerTask = client.GetAsync("/api/web/footer-config");
        var sliderTask = client.GetAsync("/api/slider");

        await Task.WhenAll(tenantTask, menusTask, navTask, footerTask, sliderTask);
        sw.Stop();

        Assert.Equal(HttpStatusCode.OK, (await tenantTask).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await menusTask).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await navTask).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await footerTask).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await sliderTask).StatusCode);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(5));
    }
}
