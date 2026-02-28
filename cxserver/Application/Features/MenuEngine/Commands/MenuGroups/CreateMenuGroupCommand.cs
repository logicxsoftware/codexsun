using cxserver.Application.Abstractions;
using cxserver.Domain.MenuEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.MenuEngine.Commands.MenuGroups;

public sealed record CreateMenuGroupCommand(
    Guid? TenantId,
    MenuGroupType Type,
    string Name,
    string Slug,
    string? Description,
    bool IsActive) : IRequest<MenuGroupItem>;

internal sealed class CreateMenuGroupCommandHandler : IRequestHandler<CreateMenuGroupCommand, MenuGroupItem>
{
    private readonly IMenuStore _store;

    public CreateMenuGroupCommandHandler(IMenuStore store)
    {
        _store = store;
    }

    public async Task<MenuGroupItem> Handle(CreateMenuGroupCommand request, CancellationToken cancellationToken)
    {
        return await _store.CreateMenuGroupAsync(
            new CreateMenuGroupInput(
                request.TenantId,
                request.Type,
                request.Name,
                request.Slug,
                request.Description,
                request.IsActive),
            cancellationToken);
    }
}

internal sealed class CreateMenuGroupCommandValidator : AbstractValidator<CreateMenuGroupCommand>
{
    public CreateMenuGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Description).MaximumLength(512);
    }
}
