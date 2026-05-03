namespace Nova.Common.Application.Clock;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
    
    DateTimeOffset Now { get; }
}