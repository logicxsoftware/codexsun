using System.Text.Json;
using MediatR;

namespace cxserver.Application.Features.ConfigurationDocuments.Queries.GetConfigurationDocument;

public sealed record GetConfigurationDocumentQuery(string NamespaceKey, string DocumentKey) : IRequest<JsonDocument?>;
