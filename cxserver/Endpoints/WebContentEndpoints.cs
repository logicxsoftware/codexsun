using cxserver.Application.Features.WebEngine.Queries.GetPublishedPage;
using MediatR;

namespace cxserver.Endpoints;

public static class WebContentEndpoints
{
    public static RouteGroupBuilder MapWebContentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/web");

        group.MapGet("/{slug}", async (string slug, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetPublishedPageQuery(slug), cancellationToken);
            return response is null ? Results.NotFound() : Results.Ok(response);
        });

        return group;
    }
}
