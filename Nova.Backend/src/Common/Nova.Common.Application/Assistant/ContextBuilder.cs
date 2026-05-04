using Nova.Common.Application.Memory;
using Nova.Common.Application.Relationships;

namespace Nova.Common.Application.Assistant;


public sealed class ContextBuilder(
    IAssistantMemory memory,
    IRelationshipContextProvider relationships)
{
    public async Task<AssistantContext> BuildAsync(
        Guid userId,
        string text,
        CancellationToken ct)
    {
        var memoryItems = await memory.SearchAsync(
            userId,
            "user communication preferences response style " + text,
            limit: 10,
            ct);

        var relevantMemory = memoryItems
            .Select(x => new ContextMemoryItem(
                x.Content,
                x.Relevance))
            .ToArray();

        var prefersShort = relevantMemory.Any(x =>
            x.Content.Contains("корот", StringComparison.OrdinalIgnoreCase) ||
            x.Content.Contains("short", StringComparison.OrdinalIgnoreCase));

        var relationship = await relationships.GetOrCreateAsync(userId, ct);

        return new AssistantContext
        {
            UserId = userId,
            RelevantMemory = relevantMemory,
            ResponseStyle = prefersShort ? ResponseStyle.Short : ResponseStyle.Normal,
            Relationship = relationship,
            AccessLevel = relationship.AccessLevel
        };
    }
}