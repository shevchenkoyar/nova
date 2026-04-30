using Nava.Modules.Memory.Contracts;

namespace Nova.Common.Application.Assistant;

public sealed class ContextBuilder(IMemoryModuleApi memory)
{
    public async Task<AssistantContext> BuildAsync(
        Guid userId,
        string text,
        CancellationToken ct)
    {
        var memoryItems = await memory.SearchAsync(
            new SearchMemoryRequest(
                userId,
                "user communication preferences response style " + text,
                Limit: 10),
            ct);

        var relevantMemory = memoryItems
            .Select(x => new ContextMemoryItem(x.Content, x.Relevance))
            .ToArray();

        var prefersShort = relevantMemory.Any(x =>
            x.Content.Contains("корот", StringComparison.OrdinalIgnoreCase) ||
            x.Content.Contains("short", StringComparison.OrdinalIgnoreCase));

        return new AssistantContext
        {
            UserId = userId,
            RelevantMemory = relevantMemory,
            ResponseStyle = prefersShort
                ? ResponseStyle.Short
                : ResponseStyle.Normal
        };
    }
}