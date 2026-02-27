using FluentValidation;

namespace cxserver.Application.Features.ConfigurationDocuments.Queries.GetConfigurationDocument;

internal sealed class GetConfigurationDocumentQueryValidator : AbstractValidator<GetConfigurationDocumentQuery>
{
    public GetConfigurationDocumentQueryValidator()
    {
        RuleFor(x => x.NamespaceKey).NotEmpty().MaximumLength(128);
        RuleFor(x => x.DocumentKey).NotEmpty().MaximumLength(256);
    }
}
