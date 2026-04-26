using Nova.Common.Domain;

namespace Nova.Common.Presentation.Idempotency;

public static class IdempotencyErrors
{
    public static readonly Error MissingKey =
        new("Idempotency.MissingKey", "Missing X-Idempotency-Key header.", ErrorType.Problem);

    public static readonly Error InvalidKey =
        new("Idempotency.InvalidKey", "Invalid X-Idempotency-Key header. Must be a GUID.", ErrorType.Problem);
}
