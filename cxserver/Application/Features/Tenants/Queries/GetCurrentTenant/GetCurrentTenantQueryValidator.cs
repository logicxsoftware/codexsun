using FluentValidation;

namespace cxserver.Application.Features.Tenants.Queries.GetCurrentTenant;

internal sealed class GetCurrentTenantQueryValidator : AbstractValidator<GetCurrentTenantQuery>
{
    public GetCurrentTenantQueryValidator()
    {
    }
}
