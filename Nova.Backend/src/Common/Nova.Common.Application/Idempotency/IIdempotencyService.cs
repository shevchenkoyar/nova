namespace Nova.Common.Application.Idempotency;

public interface IIdempotencyService
{
    Task<bool> RequestExistsAsync(Guid requestId);
    
    Task<bool> CreateRequestAsync(Guid requestId, string name);
}