namespace cxserver.Infrastructure.Persistence;

internal interface ITenantDbContextFactory
{
    ValueTask<TenantDbContext> CreateAsync(string connectionString, CancellationToken cancellationToken);
}
