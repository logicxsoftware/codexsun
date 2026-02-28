using cxserver.Domain.Common;

namespace cxserver.Domain.ContactEngine;

public sealed class ContactMessage : AggregateRoot
{
    private ContactMessage(
        Guid id,
        Guid tenantId,
        string name,
        string email,
        string? subject,
        string message,
        DateTimeOffset createdAtUtc) : base(id)
    {
        TenantId = tenantId;
        Name = name;
        Email = email;
        Subject = subject;
        Message = message;
        CreatedAtUtc = createdAtUtc;
    }

    private ContactMessage() : base(Guid.NewGuid())
    {
        Name = string.Empty;
        Email = string.Empty;
        Message = string.Empty;
    }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string? Subject { get; private set; }
    public string Message { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static ContactMessage Create(
        Guid id,
        Guid tenantId,
        string name,
        string email,
        string? subject,
        string message,
        DateTimeOffset createdAtUtc)
    {
        return new ContactMessage(
            id,
            tenantId,
            name.Trim(),
            email.Trim(),
            string.IsNullOrWhiteSpace(subject) ? null : subject.Trim(),
            message.Trim(),
            createdAtUtc);
    }
}
