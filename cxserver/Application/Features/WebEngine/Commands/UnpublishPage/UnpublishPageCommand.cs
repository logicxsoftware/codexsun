using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Commands.UnpublishPage;

public sealed record UnpublishPageCommand(Guid PageId) : IRequest<UnpublishPageResponse>;

public sealed record UnpublishPageResponse(bool Updated);

internal sealed class UnpublishPageCommandHandler : IRequestHandler<UnpublishPageCommand, UnpublishPageResponse>
{
    private readonly IWebsitePageStore _store;

    public UnpublishPageCommandHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<UnpublishPageResponse> Handle(UnpublishPageCommand request, CancellationToken cancellationToken)
    {
        var updated = await _store.UnpublishPageAsync(request.PageId, cancellationToken);
        return new UnpublishPageResponse(updated);
    }
}

internal sealed class UnpublishPageCommandValidator : AbstractValidator<UnpublishPageCommand>
{
    public UnpublishPageCommandValidator()
    {
        RuleFor(x => x.PageId).NotEmpty();
    }
}
