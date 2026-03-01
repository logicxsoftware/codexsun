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
        Assert.Equal(countsBefore["blog_categories"], countsAfter["blog_categories"]);
        Assert.Equal(countsBefore["blog_tags"], countsAfter["blog_tags"]);
        Assert.Equal(countsBefore["blog_posts"], countsAfter["blog_posts"]);
        Assert.Equal(countsBefore["blog_comments"], countsAfter["blog_comments"]);
        Assert.Equal(countsBefore["blog_likes"], countsAfter["blog_likes"]);
        Assert.Equal(countsBefore["blog_post_images"], countsAfter["blog_post_images"]);
        Assert.Equal(countsBefore["blog_post_tags"], countsAfter["blog_post_tags"]);

        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM slide_layers l LEFT JOIN slides s ON s.id = l.slide_id WHERE l.is_deleted = 0 AND s.id IS NULL") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM highlights h LEFT JOIN slides s ON s.id = h.slide_id WHERE h.is_deleted = 0 AND s.id IS NULL") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM menu_items mi LEFT JOIN menus m ON m.id = mi.menu_id WHERE mi.is_deleted = 0 AND m.id IS NULL") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM blog_posts p LEFT JOIN blog_categories c ON c.id = p.category_id WHERE p.is_deleted = 0 AND c.id IS NULL") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM blog_post_tags pt LEFT JOIN blog_posts p ON p.id = pt.post_id LEFT JOIN blog_tags t ON t.id = pt.tag_id WHERE pt.is_deleted = 0 AND (p.id IS NULL OR t.id IS NULL)") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM blog_comments c LEFT JOIN blog_posts p ON p.id = c.post_id WHERE c.is_deleted = 0 AND p.id IS NULL") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM blog_likes l LEFT JOIN blog_posts p ON p.id = l.post_id WHERE l.is_deleted = 0 AND p.id IS NULL") == 0);
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM blog_post_images i LEFT JOIN blog_posts p ON p.id = i.post_id WHERE p.id IS NULL") == 0);

        Assert.True(await ScalarIntAsync(tenantConnection, """
            SELECT COUNT(*) FROM (
                SELECT menu_id, IFNULL(parent_id, 'ROOT') AS parent_key, display_order, COUNT(*) AS c
                FROM menu_items
                WHERE is_deleted = 0
                GROUP BY menu_id, IFNULL(parent_id, 'ROOT'), display_order
                HAVING COUNT(*) > 1
            ) x
            """) == 0);

        Assert.Equal(5, await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM blog_categories WHERE is_deleted = 0 AND active = 1"));
        Assert.True(await ScalarIntAsync(tenantConnection, "SELECT COUNT(*) FROM blog_posts WHERE is_deleted = 0 AND active = 1 AND published = 1") >= 15);

        var headerTopLevel = await LoadTopLevelSlugsAsync(tenantConnection, "primary");
        Assert.Equal(["home", "shop", "services-main", "company-main", "blog-main"], headerTopLevel);

        var mobileTopLevel = await LoadTopLevelSlugsAsync(tenantConnection, "mobile-primary");
        Assert.Equal(["home-mobile", "shop-mobile", "services-main-mobile", "company-main-mobile", "blog-main-mobile"], mobileTopLevel);

        var variantCount = await ScalarIntAsync(tenantConnection, "SELECT COUNT(DISTINCT variant) FROM slides WHERE is_deleted = 0");
        var backgroundModeCount = await ScalarIntAsync(tenantConnection, "SELECT COUNT(DISTINCT background_mode) FROM slides WHERE is_deleted = 0");

        Assert.True(variantCount >= 4);
        Assert.True(backgroundModeCount >= 4);
    }

    private static async Task<Dictionary<string, int>> LoadCountsAsync(MySqlConnection connection)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var tables = new (string Table, bool SoftDelete)[]
        {
            ("menu_groups", true),
            ("menus", true),
            ("menu_items", true),
            ("web_navigation_configs", true),
            ("footer_configs", true),
            ("slider_configs", true),
            ("slides", true),
            ("slide_layers", true),
            ("highlights", true),
            ("blog_categories", true),
            ("blog_tags", true),
            ("blog_posts", true),
            ("blog_post_tags", true),
            ("blog_comments", true),
            ("blog_likes", true),
            ("blog_post_images", false),
        };

        foreach (var table in tables)
        {
            var filter = table.SoftDelete ? " WHERE is_deleted = 0" : string.Empty;
            result[table.Table] = await ScalarIntAsync(connection, $"SELECT COUNT(*) FROM {table.Table}{filter}");
        }

        return result;
    }

    private static async Task<List<string>> LoadTopLevelSlugsAsync(MySqlConnection connection, string menuSlug)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT mi.slug
            FROM menu_items mi
            INNER JOIN menus m ON m.id = mi.menu_id
            WHERE m.slug = @menuSlug
              AND m.is_deleted = 0
              AND mi.is_deleted = 0
              AND mi.parent_id IS NULL
            ORDER BY mi.display_order
            """;
        command.Parameters.AddWithValue("@menuSlug", menuSlug);

        var result = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(reader.GetString(0));
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
