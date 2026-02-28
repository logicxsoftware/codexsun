using cxserver.Application.Abstractions;
using cxserver.Domain.MenuEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.MenuItems;

public sealed record UpdateMenuItemCommand(
    Guid Id,
    Guid? ParentId,
    string Title,
    string Slug,
    string Url,
    MenuItemTarget Target,
    string? Icon,
    string? Description,
    int Order,
    bool IsActive) : IRequest<MenuNodeItem?>;

internal sealed class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, MenuNodeItem?>
{
    private readonly IMenuStore _store;

    public UpdateMenuItemCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<MenuNodeItem?> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        return await _store.UpdateMenuItemAsync(
            new UpdateMenuNodeInput(
                request.Id,
                request.ParentId,
                request.Title,
                request.Slug,
                request.Url,
                request.Target,
                request.Icon,
                request.Description,
                request.Order,
                request.IsActive),
            cancellationToken);
    }
}

internal sealed class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Url).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.Icon).MaximumLength(128);
        RuleFor(x => x.Description).MaximumLength(512);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
