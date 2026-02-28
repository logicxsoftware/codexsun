using cxserver.Domain.ContactEngine;

namespace cxserver.Application.Abstractions;

public interface IContactMessageStore
{
    Task AddAsync(ContactMessage message, CancellationToken cancellationToken);
}
