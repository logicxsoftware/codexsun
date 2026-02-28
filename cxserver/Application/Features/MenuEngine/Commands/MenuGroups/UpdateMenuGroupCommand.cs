using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.MenuGroups;

public sealed record UpdateMenuGroupCommand(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive) : IRequest<MenuGroupItem?>;

internal sealed class UpdateMenuGroupCommandHandler : IRequestHandler<UpdateMenuGroupCommand, MenuGroupItem?>
{
    private readonly IMenuStore _store;

    public UpdateMenuGroupCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<MenuGroupItem?> Handle(UpdateMenuGroupCommand request, CancellationToken cancellationToken)
    {
        return await _store.UpdateMenuGroupAsync(
            new UpdateMenuGroupInput(
                request.Id,
                request.Name,
                request.Slug,
                request.Description,
                request.IsActive),
            cancellationToken);
    }
}

internal sealed class UpdateMenuGroupCommandValidator : AbstractValidator<UpdateMenuGroupCommand>
{
    public UpdateMenuGroupCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Description).MaximumLength(512);
    }
}
