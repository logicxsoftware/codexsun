using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.WebEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Queries.GetPublishedPage;

public sealed record GetPublishedPageQuery(string Slug) : IRequest<GetPublishedPageResponse?>;

public sealed record GetPublishedPageResponse(
    string Slug,
    string Title,
    string SeoTitle,
    string SeoDescription,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<GetPublishedPageSectionResponse> Sections);

public sealed record GetPublishedPageSectionResponse(
    Guid Id,
    SectionType SectionType,
    int DisplayOrder,
    JsonElement SectionData,
    DateTimeOffset UpdatedAtUtc);

internal sealed class GetPublishedPageQueryHandler : IRequestHandler<GetPublishedPageQuery, GetPublishedPageResponse?>
{
    private readonly IWebsitePageStore _store;

    public GetPublishedPageQueryHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<GetPublishedPageResponse?> Handle(GetPublishedPageQuery request, CancellationToken cancellationToken)
    {
        var page = await _store.GetPublishedBySlugAsync(request.Slug, cancellationToken);
        if (page is null)
        {
            return null;
        }

        var sections = await _store.GetSectionsAsync(page.Id, true, cancellationToken);

        return new GetPublishedPageResponse(
            page.Slug,
            page.Title,
            page.SeoTitle,
            page.SeoDescription,
            page.UpdatedAtUtc,
            sections.OrderBy(x => x.DisplayOrder)
                .Select(x => new GetPublishedPageSectionResponse(
                    x.Id,
                    x.SectionType,
                    x.DisplayOrder,
                    x.SectionData.RootElement.Clone(),
                    x.UpdatedAtUtc))
                .ToList());
    }
}

internal sealed class GetPublishedPageQueryValidator : AbstractValidator<GetPublishedPageQuery>
{
    public GetPublishedPageQueryValidator()
    {
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(256).Matches("^[a-zA-Z0-9\\-]+$");
    }
}
