namespace cxserver.Application.Abstractions;

public sealed record TenantSession(Guid TenantId, string Identifier);
