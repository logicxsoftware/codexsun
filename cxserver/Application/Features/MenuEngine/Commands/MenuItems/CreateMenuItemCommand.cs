using cxserver.Application.Abstractions;
using cxserver.Domain.MenuEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.MenuItems;

public sealed record CreateMenuItemCommand(
    Guid MenuId,
    Guid? TenantId,
    Guid? ParentId,
    string Title,
    string Slug,
    string Url,
    MenuItemTarget Target,
    string? Icon,
    string? Description,
    int Order,
    bool IsActive) : IRequest<MenuNodeItem>;

internal sealed class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, MenuNodeItem>
{
    private readonly IMenuStore _store;

    public CreateMenuItemCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<MenuNodeItem> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        return await _store.CreateMenuItemAsync(
            new CreateMenuNodeInput(
                request.MenuId,
                request.TenantId,
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

internal sealed class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.MenuId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Url).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.Icon).MaximumLength(128);
        RuleFor(x => x.Description).MaximumLength(512);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
