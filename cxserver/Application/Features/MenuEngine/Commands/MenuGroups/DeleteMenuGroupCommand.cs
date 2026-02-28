using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.MenuGroups;

public sealed record DeleteMenuGroupCommand(Guid Id) : IRequest<bool>;

internal sealed class DeleteMenuGroupCommandHandler : IRequestHandler<DeleteMenuGroupCommand, bool>
{
    private readonly IMenuStore _store;

    public DeleteMenuGroupCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<bool> Handle(DeleteMenuGroupCommand request, CancellationToken cancellationToken)
    {
        return await _store.DeleteMenuGroupAsync(request.Id, cancellationToken);
    }
}

internal sealed class DeleteMenuGroupCommandValidator : AbstractValidator<DeleteMenuGroupCommand>
{
    public DeleteMenuGroupCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
