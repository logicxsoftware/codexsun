using FluentValidation;

namespace cxserver.Application.Features.Tenants.Commands.OnboardTenant;

internal sealed class OnboardTenantCommandValidator : AbstractValidator<OnboardTenantCommand>
{
    public OnboardTenantCommandValidator()
    {
        RuleFor(x => x.Identifier).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9\\-]+$");
        RuleFor(x => x.Domain).NotEmpty().MaximumLength(255).Matches("^[a-zA-Z0-9.-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.DatabaseName).NotEmpty().MaximumLength(128).Matches("^[a-zA-Z0-9_]+$");
        RuleFor(x => x.FeatureSettingsJson).Must(BeJsonOrNull);
        RuleFor(x => x.IsolationMetadataJson).Must(BeJsonOrNull);
    }

    private static bool BeJsonOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        try
        {
            _ = System.Text.Json.JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
