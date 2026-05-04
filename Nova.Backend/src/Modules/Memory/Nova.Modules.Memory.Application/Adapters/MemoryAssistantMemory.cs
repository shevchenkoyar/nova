using Nova.Common.Application.Memory;

namespace Nova.Modules.Memory.Application.Adapters;

public sealed class MemoryAssistantMemory(
    MemoryService memory)
    : IAssistantMemory
{
    public async Task<IReadOnlyList<AssistantMemoryItem>> SearchAsync(
        Guid userId,
        string query,
        int limit,
        CancellationToken ct)
    {
        var facts = await memory.SearchAsync(userId, query, limit, ct);

        return facts
            .Select(x => new AssistantMemoryItem(
                x.Content,
                x.Importance))
            .ToArray();
    }
}