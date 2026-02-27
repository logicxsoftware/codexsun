using System.Text.Json;
using cxserver.Application.Abstractions;
using cxserver.Domain.Configuration;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.ConfigurationStorage;

internal sealed class ConfigurationDocumentStore : IConfigurationDocumentStore
{
    private static readonly Func<TenantDbContext, string, string, IAsyncEnumerable<ConfigurationDocument>> DocumentByKeyQuery =
        EF.CompileAsyncQuery((TenantDbContext db, string namespaceKey, string documentKey) =>
            db.ConfigurationDocuments.Where(x => x.NamespaceKey == namespaceKey && x.DocumentKey == documentKey).Take(1));

    private readonly ITenantDbContextAccessor _tenantDbContextAccessor;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ConfigurationDocumentStore(ITenantDbContextAccessor tenantDbContextAccessor, IDateTimeProvider dateTimeProvider)
    {
        _tenantDbContextAccessor = tenantDbContextAccessor;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<JsonDocument?> GetAsync(string namespaceKey, string documentKey, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);

        var document = await DocumentByKeyQuery(dbContext, namespaceKey, documentKey).FirstOrDefaultAsync(cancellationToken);

        return document?.Payload;
    }

    public async Task UpsertAsync(string namespaceKey, string documentKey, JsonDocument payload, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);

        var existing = await dbContext.ConfigurationDocuments
            .AsTracking()
            .FirstOrDefaultAsync(x => x.NamespaceKey == namespaceKey && x.DocumentKey == documentKey, cancellationToken);

        if (existing is null)
        {
            var created = ConfigurationDocument.Create(
                Guid.NewGuid(),
                namespaceKey,
                documentKey,
                payload,
                _dateTimeProvider.UtcNow);

            await dbContext.ConfigurationDocuments.AddAsync(created, cancellationToken);
            return;
        }

        if (existing.IsDeleted)
        {
            existing.Restore(_dateTimeProvider.UtcNow);
        }

        existing.UpdatePayload(payload, _dateTimeProvider.UtcNow);
    }
}
