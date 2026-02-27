using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.WebEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Commands.UpdateSection;

public sealed record UpdateSectionCommand(
    Guid PageId,
    Guid SectionId,
    int DisplayOrder,
    JsonDocument SectionData,
    bool IsPublished) : IRequest<UpdateSectionResponse?>;

public sealed record UpdateSectionResponse(Guid Id, SectionType SectionType, int DisplayOrder, bool IsPublished);

internal sealed class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, UpdateSectionResponse?>
{
    private readonly IWebsitePageStore _store;
    private readonly ISectionDataValidator _validator;

    public UpdateSectionCommandHandler(IWebsitePageStore store, ISectionDataValidator validator)
    {
        _store = store;
        _validator = validator;
    }

    public async Task<UpdateSectionResponse?> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var existing = await _store.GetSectionsAsync(request.PageId, false, cancellationToken);
        var current = existing.FirstOrDefault(x => x.Id == request.SectionId);
        if (current is null)
        {
            return null;
        }

        if (!_validator.IsValid(current.SectionType, request.SectionData))
        {
            throw new InvalidOperationException("Section data is invalid.");
        }

        var updated = await _store.UpdateSectionAsync(new UpdateWebsitePageSectionInput(
            request.PageId,
            request.SectionId,
            request.DisplayOrder,
            request.SectionData,
            request.IsPublished), cancellationToken);

        return updated is null
            ? null
            : new UpdateSectionResponse(updated.Id, updated.SectionType, updated.DisplayOrder, updated.IsPublished);
    }
}

internal sealed class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
{
    public UpdateSectionCommandValidator()
    {
        RuleFor(x => x.PageId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SectionData).NotNull();
    }
}
