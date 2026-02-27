using cxserver.Infrastructure.Tenancy;

namespace cxserver.Infrastructure.Persistence;

internal sealed class TenantDbContextAccessor : ITenantDbContextAccessor, IAsyncDisposable
{
    private readonly ITenantConnectionAccessor _tenantConnectionAccessor;
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly SemaphoreSlim _gate;
    private TenantDbContext? _dbContext;

    public TenantDbContextAccessor(ITenantConnectionAccessor tenantConnectionAccessor, ITenantDbContextFactory tenantDbContextFactory)
    {
        _tenantConnectionAccessor = tenantConnectionAccessor;
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

            if (string.IsNullOrWhiteSpace(_tenantConnectionAccessor.ConnectionString))
            {
                throw new InvalidOperationException("Tenant connection is not resolved for current request.");
            }

            _dbContext = await _tenantDbContextFactory.CreateAsync(_tenantConnectionAccessor.ConnectionString, cancellationToken);
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
