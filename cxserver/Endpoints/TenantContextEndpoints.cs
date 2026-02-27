using cxserver.Application.Features.Tenants.Queries.GetCurrentTenant;
using MediatR;

namespace cxserver.Endpoints;

public static class TenantContextEndpoints
{
    public static RouteGroupBuilder MapTenantContextEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenant");

        group.MapGet("/current", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetCurrentTenantQuery(), cancellationToken);
            return Results.Ok(response);
        });

        return group;
    }
}
