namespace Nova.Modules.Relationships.Contracts;

public enum RelationshipInteractionKind
{
    Neutral = 0,
    Polite = 1,
    Helpful = 2,
    Apology = 3,

    Rude = 10,
    Aggressive = 11,
    BoundaryViolation = 12,
    DangerousRequest = 13
}