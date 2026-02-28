using System.Net.Http.Json;
using System.Text.Json;
using cxtest.TestKit;
using MySqlConnector;

namespace cxtest.integration;

public sealed class HomeDataTests
{
    [Fact]
    public async Task Home_Data_Parts_Are_Returned_With_Valid_Structure()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();
        client.DefaultRequestHeaders.Host = "localhost";

        var tenantResponse = await client.GetAsync("/api/tenant/current");
        var menusResponse = await client.GetAsync("/api/web/menus");
        var navResponse = await client.GetAsync("/api/web/navigation-config");
        var footerResponse = await client.GetAsync("/api/web/footer-config");
        var sliderResponse = await client.GetAsync("/api/slider");

        Assert.True(tenantResponse.IsSuccessStatusCode);
        Assert.True(menusResponse.IsSuccessStatusCode);
        Assert.True(navResponse.IsSuccessStatusCode);
        Assert.True(footerResponse.IsSuccessStatusCode);
        Assert.True(sliderResponse.IsSuccessStatusCode);

        var tenantJson = JsonDocument.Parse(await tenantResponse.Content.ReadAsStringAsync());
        var menusJson = JsonDocument.Parse(await menusResponse.Content.ReadAsStringAsync());
        var navJson = JsonDocument.Parse(await navResponse.Content.ReadAsStringAsync());
        var footerJson = JsonDocument.Parse(await footerResponse.Content.ReadAsStringAsync());
        var sliderJson = JsonDocument.Parse(await sliderResponse.Content.ReadAsStringAsync());

        Assert.Equal(JsonValueKind.Object, tenantJson.RootElement.ValueKind);
        Assert.Equal(JsonValueKind.Array, menusJson.RootElement.ValueKind);
        Assert.Equal(JsonValueKind.Object, navJson.RootElement.ValueKind);
        Assert.Equal(JsonValueKind.Object, footerJson.RootElement.ValueKind);
        Assert.Equal(JsonValueKind.Object, sliderJson.RootElement.ValueKind);
        Assert.True(sliderJson.RootElement.GetProperty("slides").GetArrayLength() >= 0);
    }
}
