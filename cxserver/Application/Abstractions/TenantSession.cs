namespace cxserver.Application.Abstractions;

public sealed record TenantSession(
    Guid TenantId,
    string TenantName,
    string Domain,
    string TenantDatabaseConnectionString);
