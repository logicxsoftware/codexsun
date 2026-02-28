using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Queries.GetMenusByGroup;

public sealed record GetMenusByGroupQuery(Guid MenuGroupId, bool ActiveOnly) : IRequest<IReadOnlyList<MenuItemRecord>>;

internal sealed class GetMenusByGroupQueryHandler : IRequestHandler<GetMenusByGroupQuery, IReadOnlyList<MenuItemRecord>>
{
    private readonly IMenuStore _store;

    public GetMenusByGroupQueryHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<MenuItemRecord>> Handle(GetMenusByGroupQuery request, CancellationToken cancellationToken)
    {
        return await _store.GetMenusByGroupAsync(request.MenuGroupId, request.ActiveOnly, cancellationToken);
    }
}

internal sealed class GetMenusByGroupQueryValidator : AbstractValidator<GetMenusByGroupQuery>
{
    public GetMenusByGroupQueryValidator()
    {
        RuleFor(x => x.MenuGroupId).NotEmpty();
    }
}
