using Nova.Common.Domain;

namespace Nova.Common.Presentation.Idempotency;

public sealed class IdempotencyKeyException(Error error) : Exception(error.Description)
{
    public Error Error { get; } = error;
}
