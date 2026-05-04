using Nova.Common.Application.Relationships;

namespace Nova.Common.Application.Assistant;


public sealed class AssistantContext
{
    public Guid UserId { get; init; }

    public IReadOnlyList<ContextMemoryItem> RelevantMemory { get; init; } = [];

    public ResponseStyle ResponseStyle { get; init; } = ResponseStyle.Normal;

    public AssistantRelationshipContext? Relationship { get; init; }

    public AssistantAccessLevel AccessLevel { get; init; } = AssistantAccessLevel.Full;
}