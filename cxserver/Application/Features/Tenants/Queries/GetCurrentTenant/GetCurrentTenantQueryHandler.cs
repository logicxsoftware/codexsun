using cxserver.Application.Abstractions;
using MediatR;

namespace cxserver.Application.Features.Tenants.Queries.GetCurrentTenant;

internal sealed class GetCurrentTenantQueryHandler : IRequestHandler<GetCurrentTenantQuery, GetCurrentTenantResponse>
{
    private readonly ITenantContext _tenantContext;

    public GetCurrentTenantQueryHandler(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public Task<GetCurrentTenantResponse> Handle(GetCurrentTenantQuery request, CancellationToken cancellationToken)
    {
        var current = _tenantContext.Current ?? throw new InvalidOperationException("Tenant context is not resolved for current request.");
        return Task.FromResult(new GetCurrentTenantResponse(current.TenantId, current.TenantName, current.Domain));
    }
}
