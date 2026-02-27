using System.Text.Json;

namespace cxserver.Application.Abstractions;

public interface IConfigurationDocumentStore
{
    Task<JsonDocument?> GetAsync(string namespaceKey, string documentKey, CancellationToken cancellationToken);
    Task UpsertAsync(string namespaceKey, string documentKey, JsonDocument payload, CancellationToken cancellationToken);
}
