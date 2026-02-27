using cxserver.Infrastructure.Options;
using cxserver.Infrastructure.Persistence;
using cxserver.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cxserver.Infrastructure.Persistence.DesignTime;

public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var provider = Environment.GetEnvironmentVariable("CODEXSUN_DB_PROVIDER") ?? "MariaDb";
        var connectionString = Environment.GetEnvironmentVariable("CODEXSUN_TENANT_CONNECTION")
            ?? "server=localhost;port=3306;database=codexsun_tenant;user=codexsun;password=codexsun;";

        var databaseOptions = new DatabaseOptions
        {
            Provider = provider,
            Master = new MasterDatabaseOptions { ConnectionString = "server=localhost;port=3306;database=codexsun_master;user=codexsun;password=codexsun;" },
            Tenant = new TenantDatabaseOptions { ConnectionStringTemplate = "server=localhost;port=3306;database={database};user=codexsun;password=codexsun;" }
        };

        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        DbContextOptionsConfigurator.ConfigureTenant(optionsBuilder, databaseOptions, connectionString);

        return new TenantDbContext(optionsBuilder.Options, new DateTimeProvider());
    }
}
