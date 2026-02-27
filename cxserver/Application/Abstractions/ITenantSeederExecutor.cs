namespace cxserver.Application.Abstractions;

public interface ITenantSeederExecutor
{
    Task ExecuteAsync(TenantRegistryItem tenant, CancellationToken cancellationToken);
}
