using System.Data.Common;
using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Npgsql;

namespace cxserver.Infrastructure.Onboarding;

internal sealed class TenantDatabaseCreator : ITenantDatabaseCreator
{
    private readonly DatabaseOptions _databaseOptions;

    public TenantDatabaseCreator(IOptions<DatabaseOptions> databaseOptions)
    {
        _databaseOptions = databaseOptions.Value;
    }

    public async Task CreateIfNotExistsAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        if (_databaseOptions.Provider.Equals("MariaDb", StringComparison.OrdinalIgnoreCase))
        {
            var master = new MySqlConnectionStringBuilder(_databaseOptions.Master.ConnectionString)
            {
                Pooling = true,
                MinimumPoolSize = (uint)Math.Max(0, _databaseOptions.MinPoolSize),
                MaximumPoolSize = (uint)Math.Max(1, _databaseOptions.MaxPoolSize)
            };

            await using var connection = new MySqlConnection(master.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{tenant.DatabaseName}`";
            await command.ExecuteNonQueryAsync(cancellationToken);
            return;
        }

        if (_databaseOptions.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
        {
            var root = new NpgsqlConnectionStringBuilder(_databaseOptions.Master.ConnectionString)
            {
                Database = "postgres",
                Pooling = true,
                MinPoolSize = Math.Max(0, _databaseOptions.MinPoolSize),
                MaxPoolSize = Math.Max(1, _databaseOptions.MaxPoolSize)
            };

            await using var connection = new NpgsqlConnection(root.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT 1 FROM pg_database WHERE datname = @database";
            command.Parameters.Add(new NpgsqlParameter("database", tenant.DatabaseName));
            var exists = await command.ExecuteScalarAsync(cancellationToken);

            if (exists is null)
            {
                await using var createCommand = connection.CreateCommand();
                createCommand.CommandText = $"CREATE DATABASE \"{tenant.DatabaseName}\"";
                await createCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            return;
        }

        throw new InvalidOperationException($"Unsupported database provider '{_databaseOptions.Provider}'.");
    }

    public async Task DeleteIfExistsAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        if (_databaseOptions.Provider.Equals("MariaDb", StringComparison.OrdinalIgnoreCase))
        {
            var master = new MySqlConnectionStringBuilder(_databaseOptions.Master.ConnectionString)
            {
                Pooling = true,
                MinimumPoolSize = (uint)Math.Max(0, _databaseOptions.MinPoolSize),
                MaximumPoolSize = (uint)Math.Max(1, _databaseOptions.MaxPoolSize)
            };

            await using var connection = new MySqlConnection(master.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = $"DROP DATABASE IF EXISTS `{tenant.DatabaseName}`";
            await command.ExecuteNonQueryAsync(cancellationToken);
            return;
        }

        if (_databaseOptions.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
        {
            var root = new NpgsqlConnectionStringBuilder(_databaseOptions.Master.ConnectionString)
            {
                Database = "postgres",
                Pooling = true,
                MinPoolSize = Math.Max(0, _databaseOptions.MinPoolSize),
                MaxPoolSize = Math.Max(1, _databaseOptions.MaxPoolSize)
            };

            await using var connection = new NpgsqlConnection(root.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var terminate = connection.CreateCommand();
            terminate.CommandText = "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = @database AND pid <> pg_backend_pid();";
            terminate.Parameters.Add(new NpgsqlParameter("database", tenant.DatabaseName));
            await terminate.ExecuteNonQueryAsync(cancellationToken);

            await using var dropCommand = connection.CreateCommand();
            dropCommand.CommandText = $"DROP DATABASE IF EXISTS \"{tenant.DatabaseName}\"";
            await dropCommand.ExecuteNonQueryAsync(cancellationToken);
            return;
        }

        throw new InvalidOperationException($"Unsupported database provider '{_databaseOptions.Provider}'.");
    }
}
