using System.Data;
using cxserver.Application.Abstractions;
using cxserver.Application.DependencyInjection;
using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using cxserver.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;

namespace cxtest;

public sealed class TenancyBehaviorTests
{
    [Fact]
    public async Task Startup_Seeds_Master_And_Default_Tenant()
    {
        var services = BuildServices();
        await using var provider = services.BuildServiceProvider();

        await ResetDatabasesAsync(provider, "codexsun_db", "tenant1_db");
        await RunHostedInitializationAsync(provider);

        var masterCs = GetConnectionString(provider, "Database:Master:ConnectionString");
        var tenantTemplate = GetConnectionString(provider, "Database:Tenant:ConnectionStringTemplate");
        var tenantCs = tenantTemplate.Replace("{database}", "tenant1_db", StringComparison.OrdinalIgnoreCase);

        await using var master = new MySqlConnection(masterCs);
        await master.OpenAsync();

        await using (var cmd = master.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(*) FROM tenants WHERE identifier = @identifier AND domain = @domain AND status = 1 AND is_deleted = 0";
            cmd.Parameters.AddWithValue("@identifier", "default");
            cmd.Parameters.AddWithValue("@domain", "default.localhost");
            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            Assert.True(count >= 1);
        }

        await using var tenant = new MySqlConnection(tenantCs);
        await tenant.OpenAsync();

        await using (var cmd = tenant.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(*) FROM configuration_documents WHERE namespace_key = @namespaceKey AND document_key = @documentKey AND is_deleted = 0";
            cmd.Parameters.AddWithValue("@namespaceKey", "tenant");
            cmd.Parameters.AddWithValue("@documentKey", "features");
            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            Assert.True(count >= 1);
        }
    }

    [Fact]
    public async Task OnboardTenant_Is_Idempotent_And_Provisions_Database()
    {
        var services = BuildServices();
        await using var provider = services.BuildServiceProvider();

        await ResetDatabasesAsync(provider, "codexsun_db", "tenant1_db", "tenant1_db_behavior");
        await RunHostedInitializationAsync(provider);

        var sender = provider.GetRequiredService<MediatR.ISender>();

        var databaseName = "tenant1_db_behavior";
        var command = new OnboardTenantCommand(
            "tenant-behavior",
            "tenant-behavior.localhost",
            "Tenant Behavior",
            databaseName,
            "{}",
            "{}");

        var first = await sender.Send(command);
        var second = await sender.Send(command);

        Assert.False(first.IsExisting);
        Assert.True(second.IsExisting);
        Assert.Equal(first.TenantId, second.TenantId);
        Assert.True(second.IsActive);

        var tenantTemplate = GetConnectionString(provider, "Database:Tenant:ConnectionStringTemplate");
        var tenantCs = tenantTemplate.Replace("{database}", databaseName, StringComparison.OrdinalIgnoreCase);

        await using var tenant = new MySqlConnection(tenantCs);
        await tenant.OpenAsync();

        await using var cmd = tenant.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM configuration_documents WHERE namespace_key = @namespaceKey AND document_key = @documentKey AND is_deleted = 0";
        cmd.Parameters.AddWithValue("@namespaceKey", "tenant");
        cmd.Parameters.AddWithValue("@documentKey", "features");
        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        Assert.True(count >= 1);
    }

    private static ServiceCollection BuildServices()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(GetSolutionRoot(), "cxserver", "appsettings.json"), optional: false, reloadOnChange: false)
            .Build();

        services.AddSingleton<IConfiguration>(config);
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(config);

        return services;
    }

    private static async Task RunHostedInitializationAsync(IServiceProvider provider)
    {
        var hosted = provider.GetServices<IHostedService>().ToList();
        foreach (var service in hosted)
        {
            await service.StartAsync(CancellationToken.None);
        }
    }

    private static async Task ResetDatabasesAsync(IServiceProvider provider, params string[] databases)
    {
        var masterCs = GetConnectionString(provider, "Database:Master:ConnectionString");
        var builder = new MySqlConnectionStringBuilder(masterCs) { Database = "mysql" };

        await using var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        foreach (var db in databases.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await using (var drop = connection.CreateCommand())
            {
                drop.CommandText = $"DROP DATABASE IF EXISTS `{db}`";
                await drop.ExecuteNonQueryAsync();
            }

            await using (var create = connection.CreateCommand())
            {
                create.CommandText = $"CREATE DATABASE `{db}`";
                await create.ExecuteNonQueryAsync();
            }
        }
    }

    private static string GetConnectionString(IServiceProvider provider, string key)
    {
        var config = provider.GetRequiredService<IConfiguration>();
        var environment = config["Environment"];
        Assert.False(string.IsNullOrWhiteSpace(environment));

        var value = config[$"AppEnv:{environment}:{key}"];
        Assert.False(string.IsNullOrWhiteSpace(value));
        return value!;
    }

    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "codexsun.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Solution root not found.");
    }
}
