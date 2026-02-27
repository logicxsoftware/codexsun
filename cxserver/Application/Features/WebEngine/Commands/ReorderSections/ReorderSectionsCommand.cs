using cxserver.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace cxserver.Application.Features.WebEngine.Commands.ReorderSections;

public sealed record ReorderSectionsCommand(Guid PageId, IReadOnlyList<ReorderSectionsItem> Sections) : IRequest<ReorderSectionsResponse>;

public sealed record ReorderSectionsItem(Guid SectionId, int DisplayOrder);

public sealed record ReorderSectionsResponse(bool Updated);

internal sealed class ReorderSectionsCommandHandler : IRequestHandler<ReorderSectionsCommand, ReorderSectionsResponse>
{
    private readonly IWebsitePageStore _store;

    public ReorderSectionsCommandHandler(IWebsitePageStore store)
    {
        _store = store;
    }

    public async Task<ReorderSectionsResponse> Handle(ReorderSectionsCommand request, CancellationToken cancellationToken)
    {
        var updated = await _store.ReorderSectionsAsync(
            request.PageId,
            request.Sections.Select(x => new WebsitePageSectionOrderInput(x.SectionId, x.DisplayOrder)).ToList(),
            cancellationToken);
        return new ReorderSectionsResponse(updated);
    }
}

internal sealed class ReorderSectionsCommandValidator : AbstractValidator<ReorderSectionsCommand>
{
    public ReorderSectionsCommandValidator()
    {
        RuleFor(x => x.PageId).NotEmpty();
        RuleFor(x => x.Sections).NotNull().Must(x => x.Count > 0);
        RuleForEach(x => x.Sections).SetValidator(new ReorderSectionsItemValidator());
    }
}

internal sealed class ReorderSectionsItemValidator : AbstractValidator<ReorderSectionsItem>
{
    public ReorderSectionsItemValidator()
    {
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
