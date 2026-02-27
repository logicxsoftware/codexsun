namespace cxserver.Application.Abstractions;

public interface ITenantConnectionStringBuilder
{
    Task<string> BuildAsync(string databaseName, CancellationToken cancellationToken);
}
