using Nova.Common.Application.Clock;

namespace Nova.Common.Infrastructure.Clock;

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
