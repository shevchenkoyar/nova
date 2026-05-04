namespace Nova.Common.Application.Relationships;

public sealed record RelationshipAdjustment(
    int TrustDelta,
    int WarmthDelta,
    int RespectDelta,
    int FamiliarityDelta,
    int AnnoyanceDelta,
    int OffenseDelta,
    string Reason);