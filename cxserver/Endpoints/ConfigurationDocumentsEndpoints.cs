using System.Text.Json;
using cxserver.Application.Features.ConfigurationDocuments.Commands.UpsertConfigurationDocument;
using cxserver.Application.Features.ConfigurationDocuments.Queries.GetConfigurationDocument;
using MediatR;

namespace cxserver.Endpoints;

public static class ConfigurationDocumentsEndpoints
{
    public static RouteGroupBuilder MapConfigurationDocumentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/configuration-documents");

        group.MapGet("/{namespaceKey}/{documentKey}", async (
            string namespaceKey,
            string documentKey,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var payload = await sender.Send(new GetConfigurationDocumentQuery(namespaceKey, documentKey), cancellationToken);
            return payload is null ? Results.NotFound() : Results.Ok(payload.RootElement);
        });

        group.MapPut("/{namespaceKey}/{documentKey}", async (
            string namespaceKey,
            string documentKey,
            JsonDocument payload,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            await sender.Send(new UpsertConfigurationDocumentCommand(namespaceKey, documentKey, payload), cancellationToken);
            return Results.NoContent();
        });

        var web = app.MapGroup("/api/web");
        web.MapGet("/theme", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var payload = await sender.Send(new GetConfigurationDocumentQuery("theme", "tenant"), cancellationToken);
            return payload is null ? Results.Ok(new { }) : Results.Ok(payload.RootElement);
        });

        return group;
    }
}
