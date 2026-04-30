using Nava.Modules.Memory.Contracts;

namespace Nova.Common.Application.Assistant;

public sealed class AssistantContext
{
    public Guid UserId { get; init; }

    public IReadOnlyList<ContextMemoryItem> RelevantMemory { get; init; } = [];

    public ResponseStyle ResponseStyle { get; init; } = ResponseStyle.Normal;
}

public enum ResponseStyle
{
    Normal,
    Short
}

public sealed record ContextMemoryItem(
    string Content,
    double Relevance);