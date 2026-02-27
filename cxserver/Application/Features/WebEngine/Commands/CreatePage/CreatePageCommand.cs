using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Commands.CreatePage;

public sealed record CreatePageCommand(
    string Slug,
    string Title,
    string SeoTitle,
    string SeoDescription) : IRequest<CreatePageResponse>;

public sealed record CreatePageResponse(Guid Id, string Slug, bool IsPublished);

internal sealed class CreatePageCommandHandler : IRequestHandler<CreatePageCommand, CreatePageResponse>
{
    private readonly IWebsitePageStore _store;

    public CreatePageCommandHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<CreatePageResponse> Handle(CreatePageCommand request, CancellationToken cancellationToken)
    {
        var page = await _store.CreatePageAsync(new CreateWebsitePageInput(
            request.Slug,
            request.Title,
            request.SeoTitle,
            request.SeoDescription), cancellationToken);

        return new CreatePageResponse(page.Id, page.Slug, page.IsPublished);
    }
}

internal sealed class CreatePageCommandValidator : AbstractValidator<CreatePageCommand>
{
    public CreatePageCommandValidator()
    {
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(256).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
        RuleFor(x => x.SeoTitle).NotEmpty().MaximumLength(256);
        RuleFor(x => x.SeoDescription).NotEmpty().MaximumLength(1024);
    }
}
