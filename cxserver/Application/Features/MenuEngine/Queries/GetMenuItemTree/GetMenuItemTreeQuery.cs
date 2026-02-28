using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Queries.GetMenuItemTree;

public sealed record GetMenuItemTreeQuery(Guid MenuId, bool ActiveOnly) : IRequest<IReadOnlyList<MenuNodeItem>>;

internal sealed class GetMenuItemTreeQueryHandler : IRequestHandler<GetMenuItemTreeQuery, IReadOnlyList<MenuNodeItem>>
{
    private readonly IMenuStore _store;

    public GetMenuItemTreeQueryHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<MenuNodeItem>> Handle(GetMenuItemTreeQuery request, CancellationToken cancellationToken)
    {
        return await _store.GetMenuItemTreeAsync(request.MenuId, request.ActiveOnly, cancellationToken);
    }
}

internal sealed class GetMenuItemTreeQueryValidator : AbstractValidator<GetMenuItemTreeQuery>
{
    public GetMenuItemTreeQueryValidator()
    {
        RuleFor(x => x.MenuId).NotEmpty();
    }
}
