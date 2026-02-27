using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.Configuration;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Onboarding;

internal sealed class TenantSeederExecutor : ITenantSeederExecutor
{
    private readonly ITenantDbContextFactory _tenantDbContextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TenantSeederExecutor(ITenantDbContextFactory tenantDbContextFactory, IDateTimeProvider dateTimeProvider)
    {
        _tenantDbContextFactory = tenantDbContextFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task ExecuteAsync(TenantRegistryItem tenant, CancellationToken cancellationToken)
    {
        await using var dbContext = await _tenantDbContextFactory.CreateAsync(tenant.ConnectionString, cancellationToken);

        var exists = await dbContext.ConfigurationDocuments
            .AsTracking()
            .AnyAsync(x => x.NamespaceKey == "tenant" && x.DocumentKey == "bootstrap", cancellationToken);

        if (!exists)
        {
            var payload = JsonDocument.Parse($"{{\"tenantId\":\"{tenant.TenantId:D}\",\"identifier\":\"{tenant.Identifier}\"}}");
            var doc = ConfigurationDocument.Create(Guid.NewGuid(), "tenant", "bootstrap", payload, _dateTimeProvider.UtcNow);
            await dbContext.ConfigurationDocuments.AddAsync(doc, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
