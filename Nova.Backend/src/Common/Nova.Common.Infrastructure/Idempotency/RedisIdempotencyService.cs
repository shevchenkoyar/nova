using Microsoft.Extensions.Options;
using Nova.Common.Application.Idempotency;
using StackExchange.Redis;

namespace Nova.Common.Infrastructure.Idempotency;

public sealed class RedisIdempotencyService(
    IConnectionMultiplexer redis,
    IOptions<RedisIdempotencyOptions> idempotencyOptions) : IIdempotencyService
{
    private readonly IDatabase _db = redis.GetDatabase();
    
    private string BuildKey(Guid requestId) => $"{idempotencyOptions.Value.Prefix}{requestId:N}";
    
    public async Task<bool> RequestExistsAsync(Guid requestId)
    {
        return await _db.KeyExistsAsync(BuildKey(requestId));
    }

    public async Task<bool> CreateRequestAsync(Guid requestId, string name)
    {
        return await _db.StringSetAsync(
            key: BuildKey(requestId),
            value: name,
            expiry: idempotencyOptions.Value.Ttl,
            when: When.NotExists);
    }
}
