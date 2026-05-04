using Nova.Common.Application.Relationships;
using Nova.Modules.Relationships.Contracts;

namespace Nova.Modules.Relationships.Application.Adapters;

public sealed class RelationshipContextProvider(
    IRelationshipsModuleApi relationships)
    : IRelationshipContextProvider
{
    public async Task<AssistantRelationshipContext> GetOrCreateAsync(
        Guid userId,
        CancellationToken ct)
    {
        var profile = await relationships.GetOrCreateAsync(userId, ct);

        return Map(profile);
    }

    public async Task<AssistantRelationshipContext> AdjustAsync(
        Guid userId,
        RelationshipAdjustment adjustment,
        CancellationToken ct)
    {
        var profile = await relationships.AdjustAsync(
            new AdjustRelationshipRequest(
                PersonId: userId,
                TrustDelta: adjustment.TrustDelta,
                WarmthDelta: adjustment.WarmthDelta,
                RespectDelta: adjustment.RespectDelta,
                FamiliarityDelta: adjustment.FamiliarityDelta,
                AnnoyanceDelta: adjustment.AnnoyanceDelta,
                OffenseDelta: adjustment.OffenseDelta,
                Reason: adjustment.Reason),
            ct);

        return Map(profile);
    }

    private static AssistantRelationshipContext Map(
        RelationshipProfileDto profile)
    {
        return new AssistantRelationshipContext(
            PersonId: profile.PersonId,
            Trust: profile.Trust,
            Warmth: profile.Warmth,
            Respect: profile.Respect,
            Familiarity: profile.Familiarity,
            Annoyance: profile.Annoyance,
            OffenseScore: profile.OffenseScore,
            AccessLevel: (AssistantAccessLevel)(int)profile.AccessLevel);
    }
}