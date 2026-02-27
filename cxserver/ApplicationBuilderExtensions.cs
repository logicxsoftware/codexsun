using cxserver.Middleware;

namespace cxserver;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantResolutionMiddleware>();
    }
}
