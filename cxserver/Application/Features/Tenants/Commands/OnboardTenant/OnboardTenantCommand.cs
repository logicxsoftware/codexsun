using MediatR;

namespace cxserver.Application.Features.Tenants.Commands.OnboardTenant;

public sealed record OnboardTenantCommand(
    string Identifier,
    string Name,
    string DatabaseName,
    string? FeatureSettingsJson,
    string? IsolationMetadataJson) : IRequest<Application.Abstractions.OnboardTenantResult>;
