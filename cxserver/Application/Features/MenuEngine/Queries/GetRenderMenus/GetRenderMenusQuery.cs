using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Queries.GetRenderMenus;

public sealed record GetRenderMenusQuery(bool IncludeInactive) : IRequest<IReadOnlyList<MenuRenderGroupItem>>;

internal sealed class GetRenderMenusQueryHandler : IRequestHandler<GetRenderMenusQuery, IReadOnlyList<MenuRenderGroupItem>>
{
    private readonly IMenuStore _store;
    private readonly ITenantContext _tenantContext;

    public GetRenderMenusQueryHandler(IMenuStore store, ITenantContext tenantContext)
    {
        _store = store;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<MenuRenderGroupItem>> Handle(GetRenderMenusQuery request, CancellationToken cancellationToken)
    {
        return await _store.GetRenderMenusAsync(_tenantContext.TenantId, request.IncludeInactive, cancellationToken);
    }
}

internal sealed class GetRenderMenusQueryValidator : AbstractValidator<GetRenderMenusQuery>
{
}
