namespace Nova.Common.Application.Relationships;

public sealed record AssistantRelationshipContext(
    Guid PersonId,
    int Trust,
    int Warmth,
    int Respect,
    int Familiarity,
    int Annoyance,
    int OffenseScore,
    AssistantAccessLevel AccessLevel);