namespace cxserver.Infrastructure.Persistence;

internal interface ITenantDbContextAccessor
{
    Task<TenantDbContext> GetAsync(CancellationToken cancellationToken);
}
