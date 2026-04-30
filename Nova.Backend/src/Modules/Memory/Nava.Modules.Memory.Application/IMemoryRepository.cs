using Nava.Modules.Memory.Domain;

namespace Nava.Modules.Memory.Application;

public interface IMemoryRepository
{
    Task AddAsync(MemoryFact fact, CancellationToken ct);

    Task<IReadOnlyList<MemoryFact>> SearchAsync(
        Guid userId,
        string query,
        int limit,
        CancellationToken ct);
}