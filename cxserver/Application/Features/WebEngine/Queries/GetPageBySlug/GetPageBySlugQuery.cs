using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Queries.GetPageBySlug;

public sealed record GetPageBySlugQuery(string Slug) : IRequest<GetPageBySlugResponse?>;

public sealed record GetPageBySlugResponse(
    Guid Id,
    string Slug,
    string Title,
    string SeoTitle,
    string SeoDescription,
    bool IsPublished,
    DateTimeOffset? PublishedAtUtc,
    DateTimeOffset UpdatedAtUtc);

internal sealed class GetPageBySlugQueryHandler : IRequestHandler<GetPageBySlugQuery, GetPageBySlugResponse?>
{
    private readonly IWebsitePageStore _store;

    public GetPageBySlugQueryHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<GetPageBySlugResponse?> Handle(GetPageBySlugQuery request, CancellationToken cancellationToken)
    {
        var page = await _store.GetBySlugAsync(request.Slug, cancellationToken);
        return page is null
            ? null
            : new GetPageBySlugResponse(
                page.Id,
                page.Slug,
                page.Title,
                page.SeoTitle,
                page.SeoDescription,
                page.IsPublished,
                page.PublishedAtUtc,
                page.UpdatedAtUtc);
    }
}

internal sealed class GetPageBySlugQueryValidator : AbstractValidator<GetPageBySlugQuery>
{
    public GetPageBySlugQueryValidator()
    {
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(256).Matches("^[a-zA-Z0-9\\-]+$");
    }
}
