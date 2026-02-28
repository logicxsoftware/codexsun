using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.MenuItems;

public sealed record DeleteMenuItemCommand(Guid Id) : IRequest<bool>;

internal sealed class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, bool>
{
    private readonly IMenuStore _store;

    public DeleteMenuItemCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<bool> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        return await _store.DeleteMenuItemAsync(request.Id, cancellationToken);
    }
}

internal sealed class DeleteMenuItemCommandValidator : AbstractValidator<DeleteMenuItemCommand>
{
    public DeleteMenuItemCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
