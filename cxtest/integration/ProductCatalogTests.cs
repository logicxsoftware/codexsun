using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using cxtest.TestKit;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Xunit.Sdk;

namespace cxtest.integration;

public sealed class ProductCatalogTests
{
    [Fact]
    public async Task Products_Endpoint_Supports_Pagination_And_Sorting()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_products_api_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboard = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-api",
            Domain = "products-api.localhost",
            Name = "Products Api",
            DatabaseName = "tenant_products_api_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboard.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Host = "products-api.localhost";

        var response = await client.GetAsync("/api/products?page=1&pageSize=5&sort=price_asc");
        await EnsureSuccessAsync(response);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        var pagination = json.RootElement.GetProperty("pagination");

        Assert.Equal(5, data.GetArrayLength());
        Assert.Equal(1, pagination.GetProperty("page").GetInt32());
        Assert.Equal(5, pagination.GetProperty("pageSize").GetInt32());
        Assert.Equal(50, pagination.GetProperty("totalItems").GetInt32());

        var firstPrice = data[0].GetProperty("price").GetDecimal();
        var secondPrice = data[1].GetProperty("price").GetDecimal();
        Assert.True(firstPrice <= secondPrice);
    }

    [Fact]
    public async Task Products_Endpoint_Applies_Category_And_Price_Filters()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_products_filter_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboard = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-filter",
            Domain = "products-filter.localhost",
            Name = "Products Filter",
            DatabaseName = "tenant_products_filter_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboard.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Host = "products-filter.localhost";

        var response = await client.GetAsync("/api/products?category=laptops&minPrice=70000&maxPrice=120000");
        await EnsureSuccessAsync(response);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");

        Assert.True(data.GetArrayLength() > 0);
        foreach (var item in data.EnumerateArray())
        {
            Assert.Equal("laptops", item.GetProperty("categorySlug").GetString());
            var price = item.GetProperty("price").GetDecimal();
            Assert.InRange(price, 70000m, 120000m);
        }
    }

    [Fact]
    public async Task Products_Endpoint_Is_Tenant_Isolated()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_products_iso_a_db", "tenant_products_iso_b_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboardA = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-iso-a",
            Domain = "products-iso-a.localhost",
            Name = "Products Iso A",
            DatabaseName = "tenant_products_iso_a_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboardA.EnsureSuccessStatusCode();
        var onboardAJson = JsonDocument.Parse(await onboardA.Content.ReadAsStringAsync());
        var tenantIdA = onboardAJson.RootElement.GetProperty("tenantId").GetGuid();

        var onboardB = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-iso-b",
            Domain = "products-iso-b.localhost",
            Name = "Products Iso B",
            DatabaseName = "tenant_products_iso_b_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboardB.EnsureSuccessStatusCode();

        await InsertTenantUniqueProductAsync(configuration, "tenant_products_iso_a_db", tenantIdA);

        client.DefaultRequestHeaders.Host = "products-iso-a.localhost";
        var responseA = await client.GetAsync("/api/products?search=tenant-iso-exclusive");
        await EnsureSuccessAsync(responseA);
        var jsonA = JsonDocument.Parse(await responseA.Content.ReadAsStringAsync());
        Assert.True(jsonA.RootElement.GetProperty("data").GetArrayLength() >= 1);

        client.DefaultRequestHeaders.Host = "products-iso-b.localhost";
        var responseB = await client.GetAsync("/api/products?search=tenant-iso-exclusive");
        await EnsureSuccessAsync(responseB);
        var jsonB = JsonDocument.Parse(await responseB.Content.ReadAsStringAsync());
        Assert.Equal(0, jsonB.RootElement.GetProperty("data").GetArrayLength());
    }

    [Fact]
    public async Task Product_Detail_Endpoint_Returns_Tenant_Scoped_Product()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_products_detail_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboard = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-detail",
            Domain = "products-detail.localhost",
            Name = "Products Detail",
            DatabaseName = "tenant_products_detail_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboard.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Host = "products-detail.localhost";

        var response = await client.GetAsync("/api/products/dell-latitude-5440");
        await EnsureSuccessAsync(response);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("dell-latitude-5440", json.RootElement.GetProperty("product").GetProperty("slug").GetString());
        Assert.True(json.RootElement.GetProperty("product").GetProperty("images").GetArrayLength() > 0);
        Assert.True(json.RootElement.GetProperty("product").GetProperty("specifications").EnumerateObject().Any());
        Assert.True(json.RootElement.GetProperty("relatedProducts").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Product_Detail_Endpoint_Returns_404_For_Invalid_And_CrossTenant_Slug()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_products_detail_a_db", "tenant_products_detail_b_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboardA = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-detail-a",
            Domain = "products-detail-a.localhost",
            Name = "Products Detail A",
            DatabaseName = "tenant_products_detail_a_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboardA.EnsureSuccessStatusCode();
        var onboardAJson = JsonDocument.Parse(await onboardA.Content.ReadAsStringAsync());
        var tenantIdA = onboardAJson.RootElement.GetProperty("tenantId").GetGuid();

        var onboardB = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-detail-b",
            Domain = "products-detail-b.localhost",
            Name = "Products Detail B",
            DatabaseName = "tenant_products_detail_b_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboardB.EnsureSuccessStatusCode();

        await InsertTenantUniqueProductAsync(configuration, "tenant_products_detail_a_db", tenantIdA, "cross-tenant-only", true);

        client.DefaultRequestHeaders.Host = "products-detail-b.localhost";
        var crossTenantResponse = await client.GetAsync("/api/products/cross-tenant-only");
        Assert.Equal(HttpStatusCode.NotFound, crossTenantResponse.StatusCode);

        var invalidSlugResponse = await client.GetAsync("/api/products/not-existing-slug");
        Assert.Equal(HttpStatusCode.NotFound, invalidSlugResponse.StatusCode);
    }

    [Fact]
    public async Task Product_Detail_Endpoint_Does_Not_Return_Inactive_Product()
    {
        var configuration = TestEnvironment.BuildConfiguration();
        var masterDb = new MySqlConnectionStringBuilder(TestEnvironment.GetAppEnvValue(configuration, "Database:Master:ConnectionString")).Database;
        await TestEnvironment.ResetDatabasesAsync(configuration, masterDb!, "tenant1_db", "tenant_products_detail_inactive_db");

        using var server = await TestEnvironment.CreateApiServerAsync(configuration);
        using var client = server.CreateClient();

        var onboard = await client.PostAsJsonAsync("/api/tenants/onboard", new
        {
            Identifier = "products-detail-inactive",
            Domain = "products-detail-inactive.localhost",
            Name = "Products Detail Inactive",
            DatabaseName = "tenant_products_detail_inactive_db",
            FeatureSettingsJson = "{}",
            IsolationMetadataJson = "{}",
        });
        onboard.EnsureSuccessStatusCode();
        var onboardJson = JsonDocument.Parse(await onboard.Content.ReadAsStringAsync());
        var tenantId = onboardJson.RootElement.GetProperty("tenantId").GetGuid();

        await InsertTenantUniqueProductAsync(configuration, "tenant_products_detail_inactive_db", tenantId, "inactive-product-only", false);

        client.DefaultRequestHeaders.Host = "products-detail-inactive.localhost";
        var response = await client.GetAsync("/api/products/inactive-product-only");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task InsertTenantUniqueProductAsync(IConfiguration configuration, string databaseName, Guid tenantId, string slug = "tenant-iso-exclusive", bool isActive = true)
    {
        var template = TestEnvironment.GetAppEnvValue(configuration, "Database:Tenant:ConnectionStringTemplate");
        var connectionString = template.Replace("{database}", databaseName, StringComparison.OrdinalIgnoreCase);

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var categoryId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        await using (var category = connection.CreateCommand())
        {
            category.CommandText = """
                INSERT INTO categories (id, tenant_id, name, slug, parent_id, display_order)
                VALUES (@id, @tenant_id, @name, @slug, NULL, 999)
                """;
            category.Parameters.AddWithValue("@id", categoryId);
            category.Parameters.AddWithValue("@tenant_id", tenantId);
            category.Parameters.AddWithValue("@name", "Isolation");
            category.Parameters.AddWithValue("@slug", "isolation");
            await category.ExecuteNonQueryAsync();
        }

        await using (var product = connection.CreateCommand())
        {
            product.CommandText = """
                INSERT INTO products (id, tenant_id, name, slug, description, short_description, price, compare_price, sku, stock_quantity, is_active, category_id, created_at_utc)
                VALUES (@id, @tenant_id, @name, @slug, @description, @short_description, @price, NULL, @sku, @stock_quantity, @is_active, @category_id, @created_at_utc)
                """;
            product.Parameters.AddWithValue("@id", productId);
            product.Parameters.AddWithValue("@tenant_id", tenantId);
            product.Parameters.AddWithValue("@name", slug);
            product.Parameters.AddWithValue("@slug", slug);
            product.Parameters.AddWithValue("@description", slug);
            product.Parameters.AddWithValue("@short_description", slug);
            product.Parameters.AddWithValue("@price", 100m);
            product.Parameters.AddWithValue("@sku", "ISO-1");
            product.Parameters.AddWithValue("@stock_quantity", 1);
            product.Parameters.AddWithValue("@is_active", isActive);
            product.Parameters.AddWithValue("@category_id", categoryId);
            product.Parameters.AddWithValue("@created_at_utc", DateTime.UtcNow);
            await product.ExecuteNonQueryAsync();
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        throw new XunitException($"Status {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
    }
}
