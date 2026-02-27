using System.Text.Json;
using MediatR;

namespace cxserver.Application.Features.ConfigurationDocuments.Commands.UpsertConfigurationDocument;

public sealed record UpsertConfigurationDocumentCommand(string NamespaceKey, string DocumentKey, JsonDocument Payload) : IRequest;
