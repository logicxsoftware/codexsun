using cxserver.Application.Features.Tenants.Commands.OnboardTenant;
using MediatR;

namespace cxserver.Endpoints;

public static class TenantsEndpoints
{
    public sealed record OnboardTenantRequest(
        string Identifier,
        string Name,
        string DatabaseName,
        string? FeatureSettingsJson,
        string? IsolationMetadataJson);

    public static RouteGroupBuilder MapTenantsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenants");

        group.MapPost("/onboard", async (
            OnboardTenantRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new OnboardTenantCommand(
                request.Identifier,
                request.Name,
                request.DatabaseName,
                request.FeatureSettingsJson,
                request.IsolationMetadataJson), cancellationToken);

            return Results.Ok(result);
        });

        return group;
    }
}
