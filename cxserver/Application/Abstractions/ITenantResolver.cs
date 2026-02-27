namespace cxserver.Application.Abstractions;

public interface ITenantResolver
{
    Task<TenantRegistryItem?> ResolveAsync(string host, CancellationToken cancellationToken);
}
