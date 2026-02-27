using MediatR;

namespace cxserver.Application.Features.Tenants.Queries.GetCurrentTenant;

public sealed record GetCurrentTenantQuery : IRequest<GetCurrentTenantResponse>;

public sealed record GetCurrentTenantResponse(Guid TenantId, string TenantName, string Domain);
