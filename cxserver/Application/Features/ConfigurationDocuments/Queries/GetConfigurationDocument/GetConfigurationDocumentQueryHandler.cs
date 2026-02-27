using System.Text.Json;
using cxserver.Application.Abstractions;
using MediatR;

namespace cxserver.Application.Features.ConfigurationDocuments.Queries.GetConfigurationDocument;

internal sealed class GetConfigurationDocumentQueryHandler : IRequestHandler<GetConfigurationDocumentQuery, JsonDocument?>
{
    private readonly IConfigurationDocumentStore _store;

    public GetConfigurationDocumentQueryHandler(IConfigurationDocumentStore store)
    {
        _store = store;
    }

    public Task<JsonDocument?> Handle(GetConfigurationDocumentQuery request, CancellationToken cancellationToken)
    {
        return _store.GetAsync(request.NamespaceKey, request.DocumentKey, cancellationToken);
    }
}
