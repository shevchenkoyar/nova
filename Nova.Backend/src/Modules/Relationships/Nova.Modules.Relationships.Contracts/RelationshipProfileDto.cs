namespace Nova.Modules.Relationships.Contracts;

public sealed record RelationshipProfileDto(
    Guid PersonId,
    int Trust,
    int Warmth,
    int Respect,
    int Familiarity,
    int Annoyance,
    int OffenseScore,
    RelationshipAccessLevel AccessLevel);