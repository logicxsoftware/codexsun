using cxserver.Application.Abstractions;
using cxserver.Domain.MenuEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.Menus;

public sealed record UpdateMenuCommand(
    Guid Id,
    string Name,
    string Slug,
    MenuVariant Variant,
    bool IsMegaMenu,
    int Order,
    bool IsActive) : IRequest<MenuItemRecord?>;

internal sealed class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuItemRecord?>
{
    private readonly IMenuStore _store;

    public UpdateMenuCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<MenuItemRecord?> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        return await _store.UpdateMenuAsync(
            new UpdateMenuInput(
                request.Id,
                request.Name,
                request.Slug,
                request.Variant,
                request.IsMegaMenu,
                request.Order,
                request.IsActive),
            cancellationToken);
    }
}

internal sealed class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
