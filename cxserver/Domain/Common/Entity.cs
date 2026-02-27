namespace cxserver.Domain.Common;

public abstract class Entity : IEntity
{
    public Guid Id { get; protected init; }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Entity id must not be empty.", nameof(id));
        }

        Id = id;
    }
}
