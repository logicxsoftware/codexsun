using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.WebEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Commands.AddSection;

public sealed record AddSectionCommand(
    Guid PageId,
    SectionType SectionType,
    int DisplayOrder,
    JsonDocument SectionData,
    bool IsPublished) : IRequest<AddSectionResponse?>;

public sealed record AddSectionResponse(Guid Id, SectionType SectionType, int DisplayOrder, bool IsPublished);

internal sealed class AddSectionCommandHandler : IRequestHandler<AddSectionCommand, AddSectionResponse?>
{
    private readonly IWebsitePageStore _store;
    private readonly ISectionDataValidator _validator;

    public AddSectionCommandHandler(IWebsitePageStore store, ISectionDataValidator validator)
    {
        _store = store;
        _validator = validator;
    }

    public async Task<AddSectionResponse?> Handle(AddSectionCommand request, CancellationToken cancellationToken)
    {
        if (!_validator.IsValid(request.SectionType, request.SectionData))
        {
            throw new InvalidOperationException("Section data is invalid.");
        }

        var section = await _store.AddSectionAsync(new AddWebsitePageSectionInput(
            request.PageId,
            request.SectionType,
            request.DisplayOrder,
            request.SectionData,
            request.IsPublished), cancellationToken);

        return section is null
            ? null
            : new AddSectionResponse(section.Id, section.SectionType, section.DisplayOrder, section.IsPublished);
    }
}

internal sealed class AddSectionCommandValidator : AbstractValidator<AddSectionCommand>
{
    public AddSectionCommandValidator()
    {
        RuleFor(x => x.PageId).NotEmpty();
        RuleFor(x => x.SectionType).IsInEnum();
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SectionData).NotNull();
    }
}
