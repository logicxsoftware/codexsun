using cxserver.Application.Abstractions;

namespace cxserver.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
