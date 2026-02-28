using cxserver.Application.DependencyInjection;
using cxserver.Endpoints;
using cxserver.Infrastructure.DependencyInjection;
using cxserver;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;

namespace cxtest.TestKit;

internal static class TestEnvironment
{
    public static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(GetSolutionRoot(), "cxserver", "appsettings.json"), optional: false, reloadOnChange: false)
            .Build();
    }

    public static string GetAppEnvValue(IConfiguration configuration, string key)
    {
        var environment = configuration["Environment"];
        if (string.IsNullOrWhiteSpace(environment))
        {
            throw new InvalidOperationException("Environment key is missing.");
        }

        var value = configuration[$"AppEnv:{environment}:{key}"];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing configuration value: AppEnv:{environment}:{key}");
        }

        return value;
    }

    public static string GetSolutionRoot()
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

    public static ServiceProvider BuildServiceProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        return services.BuildServiceProvider();
    }

    public static async Task ResetDatabasesAsync(IConfiguration configuration, params string[] databaseNames)
    {
        var masterCs = GetAppEnvValue(configuration, "Database:Master:ConnectionString");
        var builder = new MySqlConnectionStringBuilder(masterCs) { Database = "mysql" };

        await using var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        foreach (var name in databaseNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await using (var drop = connection.CreateCommand())
            {
                drop.CommandText = $"DROP DATABASE IF EXISTS `{name}`";
                await drop.ExecuteNonQueryAsync();
            }

            await using (var create = connection.CreateCommand())
            {
                create.CommandText = $"CREATE DATABASE `{name}`";
                await create.ExecuteNonQueryAsync();
            }
        }
    }

    public static async Task RunHostedInitializationAsync(IServiceProvider provider)
    {
        var hosted = provider.GetServices<IHostedService>().ToList();
        foreach (var service in hosted)
        {
            await service.StartAsync(CancellationToken.None);
        }
    }

    public static async Task<TestServer> CreateApiServerAsync(IConfiguration configuration)
    {
        var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services.AddSingleton(configuration);
                    services.AddRouting();
                    services.AddProblemDetails();
                    services.AddOpenApi();
                    services.AddSingleton(new ServerRuntimeInfo(DateTimeOffset.UtcNow));
                    services.AddApplication();
                    services.AddInfrastructure(configuration);
                });
                webHost.Configure(app =>
                {
                    app.UseExceptionHandler();
                    app.UseRouting();
                    app.UseTenantResolution();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHomeEndpoints();
                        endpoints.MapWebContentEndpoints();
                        endpoints.MapMenuManagementEndpoints();
                        endpoints.MapWebNavigationEndpoints();
                        endpoints.MapSliderEndpoints();
                        endpoints.MapConfigurationDocumentsEndpoints();
                        endpoints.MapTenantContextEndpoints();
                        endpoints.MapTenantsEndpoints();
                        endpoints.MapProductCatalogEndpoints();
                    });
                });
            });

        var host = await hostBuilder.StartAsync();
        return host.GetTestServer();
    }
}
