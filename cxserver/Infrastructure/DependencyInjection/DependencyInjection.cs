using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Caching;
using cxserver.Infrastructure.ConfigurationStorage;
using cxserver.Infrastructure.Identity;
using cxserver.Infrastructure.Migrations;
using cxserver.Infrastructure.Onboarding;
using cxserver.Infrastructure.Options;
using cxserver.Infrastructure.Persistence;
using cxserver.Infrastructure.Seeding;
using cxserver.Infrastructure.Tenancy;
using cxserver.Infrastructure.Time;
using Microsoft.Extensions.Options;
using cxserver.Infrastructure.WebEngine;
using cxserver.Infrastructure.MenuEngine;
using cxserver.Infrastructure.NavigationEngine;
using cxserver.Infrastructure.SliderEngine;

namespace cxserver.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var environmentSection = ResolveEnvironmentSection(configuration);

        services.Configure<DatabaseOptions>(environmentSection.GetSection(DatabaseOptions.SectionName));
        services.Configure<MultiTenancyOptions>(environmentSection.GetSection(MultiTenancyOptions.SectionName));

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddHttpContextAccessor();

        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<ITenantContextAccessor>());

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<ITenantConnectionStringBuilder, TenantConnectionStringBuilder>();
        services.AddScoped<ITenantDomainLookup, DomainTenantLookup>();
        services.AddScoped<ITenantResolver, TenantResolver>();
        services.AddScoped<ITenantMetadataCache, TenantMetadataCache>();
        services.AddScoped<ITenantFeatureCache, TenantFeatureCache>();

        services.AddDbContextPool<MasterDbContext>((sp, options) =>
        {
            var databaseOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            DbContextOptionsConfigurator.ConfigureMaster(options, databaseOptions);
        });

        services.AddScoped<ITenantRegistry, TenantRegistry>();

        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
        services.AddScoped<ITenantDbContextAccessor, TenantDbContextAccessor>();

        services.AddScoped<IConfigurationDocumentStore, ConfigurationDocumentStore>();
        services.AddScoped<IWebsitePageStore, WebsitePageStore>();
        services.AddScoped<IMenuStore, MenuStore>();
        services.AddScoped<IWebsiteNavigationStore, WebsiteNavigationStore>();
        services.AddScoped<ISliderStore, SliderStore>();
        services.AddScoped<ISectionDataValidator, SectionDataValidator>();
        services.AddScoped<IUnitOfWork, TenantUnitOfWork>();

        services.AddScoped<ITenantDatabaseCreator, TenantDatabaseCreator>();
        services.AddScoped<ITenantMigrationExecutor, TenantMigrationExecutor>();
        services.AddScoped<ITenantSeederExecutor, TenantSeederExecutor>();
        services.AddScoped<ITenantFeatureConfigurationInitializer, TenantFeatureConfigurationInitializer>();
        services.AddScoped<ITenantOnboardingCoordinator, TenantOnboardingCoordinator>();

        services.AddScoped<MasterDatabaseSeeder>();
        services.AddScoped<TenantDatabaseSeeder>();
        services.AddScoped<TenantWebsitePageSeeder>();
        services.AddScoped<TenantMenuSeeder>();
        services.AddScoped<TenantNavigationSeeder>();
        services.AddScoped<TenantSliderSeeder>();

        services.AddScoped<ITenantDatabaseMigrationService, TenantDatabaseMigrationService>();

        services.AddHostedService<DatabaseInitializationHostedService>();

        return services;
    }

    private static IConfigurationSection ResolveEnvironmentSection(IConfiguration configuration)
    {
        var environment = configuration["Environment"];
        if (string.IsNullOrWhiteSpace(environment))
        {
            throw new InvalidOperationException("Configuration key 'Environment' is required.");
        }

        var appEnvSection = configuration.GetSection("AppEnv");
        if (!appEnvSection.Exists())
        {
            throw new InvalidOperationException("Configuration section 'AppEnv' is required.");
        }

        var environmentSection = appEnvSection.GetSection(environment);
        if (!environmentSection.Exists())
        {
            throw new InvalidOperationException($"Configuration section 'AppEnv:{environment}' is missing.");
        }

        return environmentSection;
    }
}
