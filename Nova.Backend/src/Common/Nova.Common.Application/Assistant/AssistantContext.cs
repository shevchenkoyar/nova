using Nava.Modules.Memory.Contracts;
using Nova.Modules.Relationships.Contracts;

namespace Nova.Common.Application.Assistant;

public sealed class AssistantContext
{
    public Guid UserId { get; init; }

    public IReadOnlyList<ContextMemoryItem> RelevantMemory { get; init; } = [];

    public ResponseStyle ResponseStyle { get; init; } = ResponseStyle.Normal;
    
    public RelationshipProfileDto? Relationship { get; init; }
    
    public RelationshipAccessLevel AccessLevel { get; init; } = RelationshipAccessLevel.Full;
}