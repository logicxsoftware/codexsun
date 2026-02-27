using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Options;
using cxserver.Infrastructure.Tenancy;
using Microsoft.Extensions.Options;

namespace cxserver.Middleware;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _headerName;

    public TenantResolutionMiddleware(RequestDelegate next, IOptions<MultiTenancyOptions> multiTenancyOptions)
    {
        _next = next;
        _headerName = multiTenancyOptions.Value.HeaderName;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantResolver tenantResolver,
        ITenantContextAccessor tenantContextAccessor,
        ITenantConnectionAccessor tenantConnectionAccessor)
    {
        var cancellationToken = context.RequestAborted;
        if (!context.Request.Headers.TryGetValue(_headerName, out var values) || string.IsNullOrWhiteSpace(values))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant identifier header is missing." }, cancellationToken);
            return;
        }

        var resolved = await tenantResolver.ResolveAsync(values.ToString(), cancellationToken);

        if (resolved is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant not found." }, cancellationToken);
            return;
        }

        tenantContextAccessor.SetTenant(new TenantSession(resolved.TenantId, resolved.Identifier));
        tenantConnectionAccessor.SetConnectionString(resolved.ConnectionString);

        try
        {
            await _next(context);
        }
        finally
        {
            tenantConnectionAccessor.Clear();
            tenantContextAccessor.Clear();
        }
    }
}
