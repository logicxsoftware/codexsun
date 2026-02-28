using System.Data;
using cxtest.TestKit;
using MySqlConnector;

namespace cxtest.integration;

public sealed class SeederTests
{
    [Fact]
    public async Task Seeder_Is_Idempotent_And_Relations_Are_Valid()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db");

        await using (var provider = TestEnvironment.BuildServiceProvider(configuration))
        {
            await TestEnvironment.RunHostedInitializationAsync(provider);
        }

        var tenantCs = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate")
            .Replace("{database}", "tenant1_db", StringComparison.OrdinalIgnoreCase);

        await using var tenantConnection = new MySqlConnection(tenantCs);
        await tenantConnection.OpenAsync();

        var countsBefore = await LoadCountsAsync(tenantConnection);

        await using (var provider = TestEnvironment.BuildServiceProvider(configuration))
        {
            await TestEnvironment.RunHostedInitializationAsync(provider);
        }

        var countsAfter = await LoadCountsAsync(tenantConnection);

        Assert.Equal(countsBefore["slider_configs"], countsAfter["slider_configs"]);
        Assert.Equal(countsBefore["web_navigation_configs"], countsAfter["web_navigation_configs"]);
        Assert.Equal(countsBefore["footer_configs"], countsAfter["footer_configs"]);
        Assert.Equal(countsBefore["menu_groups"], countsAfter["menu_groups"]);

        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM slide_layers l LEFT JOIN slides s ON s.id = l.slide_id WHERE l.is_deleted = 0 AND s.id IS NULL") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM highlights h LEFT JOIN slides s ON s.id = h.slide_id WHERE h.is_deleted = 0 AND s.id IS NULL") == 0);

        var variantCount = await ScalarIntAsync(tenantConnection, "SELECT COUNT(DISTINCT variant) FROM slides WHERE is_deleted = 0");
        var backgroundModeCount = await ScalarIntAsync(tenantConnection, "SELECT COUNT(DISTINCT background_mode) FROM slides WHERE is_deleted = 0");

        Assert.True(variantCount >= 4);
        Assert.True(backgroundModeCount >= 4);
    }

    private static async Task<Dictionary<string, int>> LoadCountsAsync(MySqlConnection connection)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var tables = new[]
        {
            "menu_groups",
            "menus",
            "menu_items",
            "web_navigation_configs",
            "footer_configs",
            "slider_configs",
            "slides",
            "slide_layers",
            "highlights"
        };

        foreach (var table in tables)
        {
            result[table] = await ScalarIntAsync(connection, $"SELECT COUNT(*) FROM {table} WHERE is_deleted = 0");
        }

        return result;
    }

    private static async Task<int> ScalarIntAsync(MySqlConnection connection, string sql)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        var value = await command.ExecuteScalarAsync();
        return Convert.ToInt32(value);
    }
}
