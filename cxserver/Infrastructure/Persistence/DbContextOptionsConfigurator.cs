using cxserver.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace cxserver.Infrastructure.Persistence;

internal static class DbContextOptionsConfigurator
{
    public static void ConfigureMaster(DbContextOptionsBuilder optionsBuilder, DatabaseOptions databaseOptions)
    {
        Configure(optionsBuilder, databaseOptions, databaseOptions.Master.ConnectionString, "__EFMigrationsHistory_Master");
    }

    public static void ConfigureTenant(DbContextOptionsBuilder optionsBuilder, DatabaseOptions databaseOptions, string connectionString)
    {
        Configure(optionsBuilder, databaseOptions, connectionString, "__EFMigrationsHistory_Tenant");
    }

    private static void Configure(
        DbContextOptionsBuilder optionsBuilder,
        DatabaseOptions databaseOptions,
        string connectionString,
        string historyTable)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        if (databaseOptions.Provider.Equals("MariaDb", StringComparison.OrdinalIgnoreCase))
        {
            var version = ParseMariaDbVersion(databaseOptions.MariaDbServerVersion);

            optionsBuilder.UseMySql(
                connectionString,
                version,
                mysql =>
                {
                    mysql.EnableRetryOnFailure();
                    mysql.MigrationsHistoryTable(historyTable);
                    mysql.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                });
            return;
        }

        if (databaseOptions.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseNpgsql(
                connectionString,
                npgsql =>
                {
                    npgsql.EnableRetryOnFailure();
                    npgsql.MigrationsHistoryTable(historyTable);
                });
            return;
        }

        throw new InvalidOperationException($"Unsupported database provider '{databaseOptions.Provider}'.");
    }

    private static MariaDbServerVersion ParseMariaDbVersion(string version)
    {
        if (!Version.TryParse(version, out var parsed))
        {
            throw new InvalidOperationException("MariaDB server version configuration is invalid.");
        }

        return new MariaDbServerVersion(parsed);
    }
}
