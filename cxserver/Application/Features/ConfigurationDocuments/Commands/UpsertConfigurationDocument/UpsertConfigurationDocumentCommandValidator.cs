using FluentValidation;

namespace cxserver.Application.Features.ConfigurationDocuments.Commands.UpsertConfigurationDocument;

internal sealed class UpsertConfigurationDocumentCommandValidator : AbstractValidator<UpsertConfigurationDocumentCommand>
{
    public UpsertConfigurationDocumentCommandValidator()
    {
        RuleFor(x => x.NamespaceKey).NotEmpty().MaximumLength(128);
        RuleFor(x => x.DocumentKey).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Payload).NotNull();
    }
}
