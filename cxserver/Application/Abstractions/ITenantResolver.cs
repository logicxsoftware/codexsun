namespace cxserver.Application.Abstractions;

public interface ITenantResolver
{
    Task<TenantRegistryItem?> ResolveAsync(string identifier, CancellationToken cancellationToken);
}
