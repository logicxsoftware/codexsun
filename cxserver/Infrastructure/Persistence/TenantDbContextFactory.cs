using System.Collections.Concurrent;
using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace cxserver.Infrastructure.Persistence;

internal sealed class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly DatabaseOptions _databaseOptions;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ConcurrentDictionary<string, DbContextOptions<TenantDbContext>> _optionsCache;

    public TenantDbContextFactory(IOptions<DatabaseOptions> databaseOptions, IDateTimeProvider dateTimeProvider)
    {
        _databaseOptions = databaseOptions.Value;
        _dateTimeProvider = dateTimeProvider;
        _optionsCache = new ConcurrentDictionary<string, DbContextOptions<TenantDbContext>>(StringComparer.Ordinal);
    }

    public ValueTask<TenantDbContext> CreateAsync(string connectionString, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = _optionsCache.GetOrAdd(connectionString, BuildOptions);
        return ValueTask.FromResult(new TenantDbContext(options, _dateTimeProvider));
    }

    private DbContextOptions<TenantDbContext> BuildOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        DbContextOptionsConfigurator.ConfigureTenant(optionsBuilder, _databaseOptions, connectionString);
        return optionsBuilder.Options;
    }
}
