using cxserver.Application.Abstractions;
using cxserver.Domain.MenuEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.Menus;

public sealed record CreateMenuCommand(
    Guid MenuGroupId,
    Guid? TenantId,
    string Name,
    string Slug,
    MenuVariant Variant,
    bool IsMegaMenu,
    int Order,
    bool IsActive) : IRequest<MenuItemRecord>;

internal sealed class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuItemRecord>
{
    private readonly IMenuStore _store;

    public CreateMenuCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<MenuItemRecord> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        return await _store.CreateMenuAsync(
            new CreateMenuInput(
                request.MenuGroupId,
                request.TenantId,
                request.Name,
                request.Slug,
                request.Variant,
                request.IsMegaMenu,
                request.Order,
                request.IsActive),
            cancellationToken);
    }
}

internal sealed class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuCommandValidator()
    {
        RuleFor(x => x.MenuGroupId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
