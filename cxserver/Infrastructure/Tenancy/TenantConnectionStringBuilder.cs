using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Npgsql;

namespace cxserver.Infrastructure.Tenancy;

internal sealed class TenantConnectionStringBuilder : ITenantConnectionStringBuilder
{
    private readonly DatabaseOptions _databaseOptions;

    public TenantConnectionStringBuilder(IOptions<DatabaseOptions> databaseOptions)
    {
        _databaseOptions = databaseOptions.Value;
    }

    public Task<string> BuildAsync(string databaseName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is required.", nameof(databaseName));
        }

        var template = _databaseOptions.Tenant.ConnectionStringTemplate;

        if (string.IsNullOrWhiteSpace(template))
        {
            throw new InvalidOperationException("Tenant connection string template is not configured.");
        }

        if (_databaseOptions.Provider.Equals("MariaDb", StringComparison.OrdinalIgnoreCase))
        {
            var builder = new MySqlConnectionStringBuilder(template.Replace("{database}", databaseName, StringComparison.OrdinalIgnoreCase))
            {
                Database = databaseName,
                Pooling = true,
                MinimumPoolSize = (uint)Math.Max(0, _databaseOptions.MinPoolSize),
                MaximumPoolSize = (uint)Math.Max(1, _databaseOptions.MaxPoolSize)
            };

            return Task.FromResult(builder.ConnectionString);
        }

        if (_databaseOptions.Provider.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase))
        {
            var builder = new NpgsqlConnectionStringBuilder(template.Replace("{database}", databaseName, StringComparison.OrdinalIgnoreCase))
            {
                Database = databaseName,
                Pooling = true,
                MinPoolSize = Math.Max(0, _databaseOptions.MinPoolSize),
                MaxPoolSize = Math.Max(1, _databaseOptions.MaxPoolSize)
            };

            return Task.FromResult(builder.ConnectionString);
        }

        throw new InvalidOperationException($"Unsupported database provider '{_databaseOptions.Provider}'.");
    }
}
