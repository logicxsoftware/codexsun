using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.MenuItems;

public sealed record ReorderMenuItemsCommand(Guid MenuId, IReadOnlyList<ReorderMenuItemsEntry> Orders) : IRequest<bool>;

public sealed record ReorderMenuItemsEntry(Guid ItemId, Guid? ParentId, int Order);

internal sealed class ReorderMenuItemsCommandHandler : IRequestHandler<ReorderMenuItemsCommand, bool>
{
    private readonly IMenuStore _store;

    public ReorderMenuItemsCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<bool> Handle(ReorderMenuItemsCommand request, CancellationToken cancellationToken)
    {
        var orderList = request.Orders.Select(x => new MenuNodeOrderInput(x.ItemId, x.ParentId, x.Order)).ToList();
        return await _store.ReorderMenuItemsAsync(request.MenuId, orderList, cancellationToken);
    }
}

internal sealed class ReorderMenuItemsCommandValidator : AbstractValidator<ReorderMenuItemsCommand>
{
    public ReorderMenuItemsCommandValidator()
    {
        RuleFor(x => x.MenuId).NotEmpty();
        RuleFor(x => x.Orders).NotNull();
        RuleForEach(x => x.Orders).SetValidator(new ReorderMenuItemsEntryValidator());
    }
}

internal sealed class ReorderMenuItemsEntryValidator : AbstractValidator<ReorderMenuItemsEntry>
{
    public ReorderMenuItemsEntryValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
