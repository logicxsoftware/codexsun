namespace cxserver.Endpoints;

public sealed record ServerRuntimeInfo(DateTimeOffset StartedAtLocal);

public static class HomeEndpoints
{
    public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", (ServerRuntimeInfo runtimeInfo) =>
        {
            var html = $@"<!doctype html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <title>cxserver</title>
    <style>
        body {{ font-family: Segoe UI, Arial, sans-serif; margin: 40px; background: #f6f8fb; color: #1f2937; }}
        .card {{ max-width: 700px; background: #ffffff; border: 1px solid #d1d5db; border-radius: 12px; padding: 24px; }}
        h1 {{ margin: 0 0 12px 0; font-size: 28px; }}
        p {{ margin: 8px 0; font-size: 16px; }}
        .stamp {{ font-weight: 600; color: #0f766e; }}
    </style>
</head>
<body>
    <div class=""card"">
        <h1>Welcome to cxserver</h1>
        <p>Service is running successfully.</p>
        <p>Started at: <span class=""stamp"">{runtimeInfo.StartedAtLocal:yyyy-MM-dd HH:mm:ss zzz}</span></p>
    </div>
</body>
</html>";

            return Results.Content(html, "text/html");
        });

        return app;
    }
}
