using System.Data;
using cxtest.TestKit;
using MySqlConnector;

namespace cxtest.integration;

public sealed class MigrationTests
{
    [Fact]
    public async Task Required_Tables_And_Constraints_Exist()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        var tenantTemplate = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate");
        var tenantDb = new MySqlConnectionStringBuilder(tenantTemplate.Replace("{database}", "tenant1_db", StringComparison.OrdinalIgnoreCase)).Database;

        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, tenantDb!);

        await using var provider = TestEnvironment.BuildServiceProvider(configuration);
        await TestEnvironment.RunHostedInitializationAsync(provider);

        await using var tenantConnection = new MySqlConnection(tenantTemplate.Replace("{database}", "tenant1_db", StringComparison.OrdinalIgnoreCase));
        await tenantConnection.OpenAsync();

        var tables = await SelectSetAsync(tenantConnection, """
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = DATABASE()
            """);

        var expectedTables = new[]
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

        foreach (var table in expectedTables)
        {
            Assert.Contains(table, tables);
        }

        var foreignKeys = await SelectSetAsync(tenantConnection, """
            SELECT constraint_name
            FROM information_schema.table_constraints
            WHERE table_schema = DATABASE() AND constraint_type = 'FOREIGN KEY'
            """);

        Assert.Contains("fk_menus_menu_groups_menu_group_id", foreignKeys);
        Assert.Contains("fk_menu_items_menus_menu_id", foreignKeys);
        Assert.Contains("fk_slides_slider_configs_slider_config_id", foreignKeys);
        Assert.Contains("fk_slide_layers_slides_slide_id", foreignKeys);
        Assert.Contains("fk_highlights_slides_slide_id", foreignKeys);

        var uniqueIndexes = await SelectSetAsync(tenantConnection, """
            SELECT index_name
            FROM information_schema.statistics
            WHERE table_schema = DATABASE() AND non_unique = 0
            """);

        Assert.Contains("ux_menu_groups_tenant_slug_soft_delete", uniqueIndexes);
        Assert.Contains("ux_menus_tenant_group_slug_soft_delete", uniqueIndexes);
        Assert.Contains("ux_menu_items_menu_slug_soft_delete", uniqueIndexes);
        Assert.Contains("ux_slider_configs_tenant_soft_delete", uniqueIndexes);
        Assert.Contains("ux_highlights_order_soft_delete", uniqueIndexes);
    }

    private static async Task<HashSet<string>> SelectSetAsync(MySqlConnection connection, string sql)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (await reader.ReadAsync())
        {
            set.Add(reader.GetString(0));
        }

        return set;
    }
}
