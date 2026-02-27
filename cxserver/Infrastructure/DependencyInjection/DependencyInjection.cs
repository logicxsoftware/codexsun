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

namespace cxserver.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<MultiTenancyOptions>(configuration.GetSection(MultiTenancyOptions.SectionName));

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddHttpContextAccessor();

        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<ITenantContextAccessor>());
        services.AddScoped<ITenantConnectionAccessor, TenantConnectionAccessor>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<ITenantConnectionStringBuilder, TenantConnectionStringBuilder>();
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
        services.AddScoped<IUnitOfWork, TenantUnitOfWork>();

        services.AddScoped<ITenantDatabaseCreator, TenantDatabaseCreator>();
        services.AddScoped<ITenantMigrationExecutor, TenantMigrationExecutor>();
        services.AddScoped<ITenantSeederExecutor, TenantSeederExecutor>();
        services.AddScoped<ITenantFeatureConfigurationInitializer, TenantFeatureConfigurationInitializer>();
        services.AddScoped<ITenantOnboardingCoordinator, TenantOnboardingCoordinator>();

        services.AddScoped<MasterDatabaseSeeder>();
        services.AddScoped<TenantDatabaseSeeder>();

        services.AddScoped<ITenantDatabaseMigrationService, TenantDatabaseMigrationService>();

        services.AddHostedService<DatabaseInitializationHostedService>();

        return services;
    }
}
