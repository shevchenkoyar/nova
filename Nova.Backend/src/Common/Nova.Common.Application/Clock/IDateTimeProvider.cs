namespace Nova.Common.Application.Clock;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}