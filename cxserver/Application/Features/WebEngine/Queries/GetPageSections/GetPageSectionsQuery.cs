using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.WebEngine;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Queries.GetPageSections;

public sealed record GetPageSectionsQuery(Guid PageId, bool PublishedOnly) : IRequest<IReadOnlyList<GetPageSectionsItemResponse>>;

public sealed record GetPageSectionsItemResponse(
    Guid Id,
    SectionType SectionType,
    int DisplayOrder,
    JsonElement SectionData,
    bool IsPublished,
    DateTimeOffset UpdatedAtUtc);

internal sealed class GetPageSectionsQueryHandler : IRequestHandler<GetPageSectionsQuery, IReadOnlyList<GetPageSectionsItemResponse>>
{
    private readonly IWebsitePageStore _store;

    public GetPageSectionsQueryHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<GetPageSectionsItemResponse>> Handle(GetPageSectionsQuery request, CancellationToken cancellationToken)
    {
        var sections = await _store.GetSectionsAsync(request.PageId, request.PublishedOnly, cancellationToken);
        return sections
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new GetPageSectionsItemResponse(
                x.Id,
                x.SectionType,
                x.DisplayOrder,
                x.SectionData.RootElement.Clone(),
                x.IsPublished,
                x.UpdatedAtUtc))
            .ToList();
    }
}

internal sealed class GetPageSectionsQueryValidator : AbstractValidator<GetPageSectionsQuery>
{
    public GetPageSectionsQueryValidator()
    {
        RuleFor(x => x.PageId).NotEmpty();
    }
}
