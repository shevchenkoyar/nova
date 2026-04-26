using JetBrains.Annotations;

namespace Nova.Common.Infrastructure.Idempotency;

public sealed class RedisIdempotencyOptions
{
    public string Prefix { get; [UsedImplicitly] set; }
    
    public TimeSpan Ttl { get; [UsedImplicitly] set; }
}
