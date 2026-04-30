using Nava.Modules.Memory.Domain;

namespace Nava.Modules.Memory.Application;

public sealed class InMemoryMemoryRepository : IMemoryRepository
{
    private readonly List<MemoryFact> _facts = [];

    public Task AddAsync(MemoryFact fact, CancellationToken ct)
    {
        _facts.Add(fact);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<MemoryFact>> SearchAsync(
        Guid userId,
        string query,
        int limit,
        CancellationToken ct)
    {
        var result = _facts
            .Where(x => x.UserId == userId)
            .Where(x =>
                x.Content.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Content.Contains("корот", StringComparison.OrdinalIgnoreCase) ||
                x.Content.Contains("short", StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .ToArray();

        return Task.FromResult<IReadOnlyList<MemoryFact>>(result);
    }
}