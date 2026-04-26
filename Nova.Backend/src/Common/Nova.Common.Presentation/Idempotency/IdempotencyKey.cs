using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Nova.Common.Presentation.Idempotency;

public readonly record struct IdempotencyKey(Guid Value)
{
    public static implicit operator Guid(IdempotencyKey key) => key.Value;
    
    public override string ToString() => Value.ToString();
    
    [UsedImplicitly]
    public static ValueTask<IdempotencyKey> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        if (!context.Request.Headers.TryGetValue(IdempotencyHeaders.IdempotencyKey, out StringValues value) ||
            string.IsNullOrWhiteSpace(value))
        {
            throw new IdempotencyKeyException(IdempotencyErrors.MissingKey);
        }

        if (!Guid.TryParse(value.ToString(), out Guid guid))
        {
            throw new IdempotencyKeyException(IdempotencyErrors.InvalidKey);
        }

        return ValueTask.FromResult(new IdempotencyKey(guid));
    }
}
