using Nava.Modules.Memory.Contracts;
using Nova.Common.Application.Memory;

namespace Nava.Modules.Memory.Application.Adapters;

public sealed class MemoryAssistantMemory(
    IMemoryModuleApi memory)
    : IAssistantMemory
{
    public async Task<IReadOnlyList<AssistantMemoryItem>> SearchAsync(
        Guid userId,
        string query,
        int limit,
        CancellationToken ct)
    {
        var result = await memory.SearchAsync(
            new SearchMemoryRequest(
                UserId: userId,
                Query: query,
                Limit: limit),
            ct);

        return result
            .Select(x => new AssistantMemoryItem(
                x.Content,
                x.Relevance))
            .ToArray();
    }
}