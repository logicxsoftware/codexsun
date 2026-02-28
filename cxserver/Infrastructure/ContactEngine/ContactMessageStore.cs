using cxserver.Application.Abstractions;
using cxserver.Domain.ContactEngine;
using cxserver.Infrastructure.Persistence;

namespace cxserver.Infrastructure.ContactEngine;

internal sealed class ContactMessageStore : IContactMessageStore
{
    private readonly ITenantDbContextAccessor _dbContextAccessor;

    public ContactMessageStore(ITenantDbContextAccessor dbContextAccessor)
    {
        _dbContextAccessor = dbContextAccessor;
    }

    public async Task AddAsync(ContactMessage message, CancellationToken cancellationToken)
    {
        var dbContext = await _dbContextAccessor.GetAsync(cancellationToken);
        await dbContext.ContactMessages.AddAsync(message, cancellationToken);
    }
}
