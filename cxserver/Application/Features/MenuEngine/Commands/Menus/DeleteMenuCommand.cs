using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.Menus;

public sealed record DeleteMenuCommand(Guid Id) : IRequest<bool>;

internal sealed class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
{
    private readonly IMenuStore _store;

    public DeleteMenuCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        return await _store.DeleteMenuAsync(request.Id, cancellationToken);
    }
}

internal sealed class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
{
    public DeleteMenuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
