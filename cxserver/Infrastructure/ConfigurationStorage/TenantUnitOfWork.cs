using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Persistence;

namespace cxserver.Infrastructure.ConfigurationStorage;

internal sealed class TenantUnitOfWork : IUnitOfWork
{
    private readonly ITenantDbContextAccessor _tenantDbContextAccessor;

    public TenantUnitOfWork(ITenantDbContextAccessor tenantDbContextAccessor)
    {
        _tenantDbContextAccessor = tenantDbContextAccessor;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
