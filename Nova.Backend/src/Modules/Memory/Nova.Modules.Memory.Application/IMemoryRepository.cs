using Nova.Modules.Memory.Domain;

namespace Nova.Modules.Memory.Application;

public interface IMemoryRepository
{
    Task AddAsync(
        MemoryFact fact,
        CancellationToken ct);

    Task<IReadOnlyList<MemoryFact>> SearchAsync(
        Guid userId,
        string query,
        float[] embedding,
        int limit,
        CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}