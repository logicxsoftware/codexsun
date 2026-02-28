using cxserver.Application.Abstractions;
using cxserver.Domain.MenuEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Queries.GetMenuGroups;

public sealed record GetMenuGroupsQuery(bool IncludeGlobal, bool ActiveOnly) : IRequest<IReadOnlyList<MenuGroupItem>>;

internal sealed class GetMenuGroupsQueryHandler : IRequestHandler<GetMenuGroupsQuery, IReadOnlyList<MenuGroupItem>>
{
    private readonly IMenuStore _store;
    private readonly ITenantContext _tenantContext;

    public GetMenuGroupsQueryHandler(IMenuStore store, ITenantContext tenantContext)
    {
        _store = store;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<MenuGroupItem>> Handle(GetMenuGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _store.GetMenuGroupsAsync(_tenantContext.TenantId, request.IncludeGlobal, request.ActiveOnly, cancellationToken);
    }
}

internal sealed class GetMenuGroupsQueryValidator : AbstractValidator<GetMenuGroupsQuery>
{
}
