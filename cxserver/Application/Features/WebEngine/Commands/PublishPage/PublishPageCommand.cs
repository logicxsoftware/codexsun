using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Commands.PublishPage;

public sealed record PublishPageCommand(Guid PageId) : IRequest<PublishPageResponse>;

public sealed record PublishPageResponse(bool Updated);

internal sealed class PublishPageCommandHandler : IRequestHandler<PublishPageCommand, PublishPageResponse>
{
    private readonly IWebsitePageStore _store;

    public PublishPageCommandHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<PublishPageResponse> Handle(PublishPageCommand request, CancellationToken cancellationToken)
    {
        var updated = await _store.PublishPageAsync(request.PageId, cancellationToken);
        return new PublishPageResponse(updated);
    }
}

internal sealed class PublishPageCommandValidator : AbstractValidator<PublishPageCommand>
{
    public PublishPageCommandValidator()
    {
        RuleFor(x => x.PageId).NotEmpty();
    }
}
