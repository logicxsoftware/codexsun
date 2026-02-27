using cxserver.Infrastructure.Options;
using cxserver.Infrastructure.Persistence;
using cxserver.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cxserver.Infrastructure.Persistence.DesignTime;

public sealed class MasterDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(string[] args)
    {
        var provider = Environment.GetEnvironmentVariable("CODEXSUN_DB_PROVIDER") ?? "MariaDb";
        var connectionString = Environment.GetEnvironmentVariable("CODEXSUN_MASTER_CONNECTION")
            ?? "server=localhost;port=3306;database=codexsun_master;user=codexsun;password=codexsun;";

        var databaseOptions = new DatabaseOptions
        {
            Provider = provider,
            Master = new MasterDatabaseOptions { ConnectionString = connectionString },
            Tenant = new TenantDatabaseOptions { ConnectionStringTemplate = "server=localhost;port=3306;database={database};user=codexsun;password=codexsun;" }
        };

        var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();
        DbContextOptionsConfigurator.ConfigureMaster(optionsBuilder, databaseOptions);

        return new MasterDbContext(optionsBuilder.Options, new DateTimeProvider());
    }
}
