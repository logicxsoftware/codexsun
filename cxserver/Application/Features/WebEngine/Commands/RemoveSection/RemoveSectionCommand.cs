using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Commands.RemoveSection;

public sealed record RemoveSectionCommand(Guid PageId, Guid SectionId) : IRequest<RemoveSectionResponse>;

public sealed record RemoveSectionResponse(bool Updated);

internal sealed class RemoveSectionCommandHandler : IRequestHandler<RemoveSectionCommand, RemoveSectionResponse>
{
    private readonly IWebsitePageStore _store;

    public RemoveSectionCommandHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<RemoveSectionResponse> Handle(RemoveSectionCommand request, CancellationToken cancellationToken)
    {
        var updated = await _store.RemoveSectionAsync(request.PageId, request.SectionId, cancellationToken);
        return new RemoveSectionResponse(updated);
    }
}

internal sealed class RemoveSectionCommandValidator : AbstractValidator<RemoveSectionCommand>
{
    public RemoveSectionCommandValidator()
    {
        RuleFor(x => x.PageId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
    }
}
