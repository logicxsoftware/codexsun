namespace cxserver.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAtUtc { get; }
    void Delete(DateTimeOffset deletedAtUtc);
    void Restore(DateTimeOffset restoredAtUtc);
}
