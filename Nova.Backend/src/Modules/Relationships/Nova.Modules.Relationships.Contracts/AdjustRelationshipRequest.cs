namespace Nova.Modules.Relationships.Contracts;

public sealed record AdjustRelationshipRequest(
    Guid PersonId,
    int TrustDelta,
    int WarmthDelta,
    int RespectDelta,
    int FamiliarityDelta,
    int AnnoyanceDelta,
    int OffenseDelta,
    string Reason);