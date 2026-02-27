using cxserver.Application.Abstractions;

namespace cxserver.Middleware;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantResolver tenantResolver,
        ITenantContextAccessor tenantContextAccessor)
    {
        if (IsBypassedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var cancellationToken = context.RequestAborted;
        var host = GetValidatedHost(context.Request);
        if (host is null)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "InvalidHost", "Host header is invalid.", cancellationToken);
            return;
        }

        var resolved = await tenantResolver.ResolveAsync(host, cancellationToken);

        if (resolved is null)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "UnknownTenant", "Tenant domain is not registered.", cancellationToken);
            return;
        }

        if (!resolved.IsActive)
        {
            await WriteProblemAsync(context, StatusCodes.Status403Forbidden, "InactiveTenant", "Tenant is inactive.", cancellationToken);
            return;
        }

        tenantContextAccessor.SetTenant(new TenantSession(
            resolved.TenantId,
            resolved.Name,
            resolved.Domain,
            resolved.ConnectionString));

        try
        {
            await _next(context);
        }
        finally
        {
            tenantContextAccessor.Clear();
        }
    }

    private static bool IsBypassedPath(PathString path)
    {
        return path == "/"
               || path.StartsWithSegments("/health")
               || path.StartsWithSegments("/alive")
               || path.StartsWithSegments("/openapi")
               || path.StartsWithSegments("/api/tenants");
    }

    private static string? GetValidatedHost(HttpRequest request)
    {
        if (request.Headers.TryGetValue("Host", out var hostValues))
        {
            if (hostValues.Count != 1)
            {
                return null;
            }

            var hostHeader = hostValues[0];
            if (string.IsNullOrWhiteSpace(hostHeader) || hostHeader.Contains(','))
            {
                return null;
            }
        }

        if (!request.Host.HasValue)
        {
            return null;
        }

        var host = request.Host.Host;
        if (string.IsNullOrWhiteSpace(host))
        {
            return null;
        }

        return host;
    }

    private static Task WriteProblemAsync(HttpContext context, int status, string code, string detail, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new
        {
            error = code,
            detail
        }, cancellationToken);
    }
}
