using cxserver.Infrastructure.Tenancy;
using cxserver.Application.Abstractions;

namespace cxserver.Infrastructure.Persistence;

internal sealed class TenantDbContextAccessor : ITenantDbContextAccessor, IAsyncDisposable
{
    private readonly ITenantContext _tenantContext;
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly SemaphoreSlim _gate;
    private TenantDbContext? _dbContext;

    public TenantDbContextAccessor(ITenantContext tenantContext, ITenantDbContextFactory tenantDbContextFactory)
    {
        _tenantContext = tenantContext;
        _tenantDbContextFactory = tenantDbContextFactory;
        _gate = new SemaphoreSlim(1, 1);
    }

    public async Task<TenantDbContext> GetAsync(CancellationToken cancellationToken)
    {
        if (_dbContext is not null)
        {
            return _dbContext;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_dbContext is not null)
            {
                return _dbContext;
            }

            if (string.IsNullOrWhiteSpace(_tenantContext.TenantDatabaseConnectionString))
            {
                throw new InvalidOperationException("Tenant connection is not resolved for current request.");
            }

            _dbContext = await _tenantDbContextFactory.CreateAsync(_tenantContext.TenantDatabaseConnectionString, cancellationToken);
            return _dbContext;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }

        _gate.Dispose();
    }
}
