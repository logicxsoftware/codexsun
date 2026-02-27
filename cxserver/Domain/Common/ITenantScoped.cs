namespace cxserver.Domain.Common;

public interface ITenantScoped
{
    string TenantId { get; }
}
