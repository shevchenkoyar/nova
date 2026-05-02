using Nova.Modules.Relationships.Contracts;
using Nova.Modules.Relationships.Domain;

namespace Nova.Modules.Relationships.Application;

public sealed class RelationshipsModuleApi(
    IRelationshipProfileRepository repository)
    : IRelationshipsModuleApi
{
    public async Task<RelationshipProfileDto> GetOrCreateAsync(
        Guid personId,
        CancellationToken ct)
    {
        var profile = await GetOrCreateProfileAsync(personId, ct);

        await repository.SaveChangesAsync(ct);

        return Map(profile);
    }

    public async Task<RelationshipProfileDto> RegisterInteractionAsync(
        RegisterInteractionRequest request,
        CancellationToken ct)
    {
        var profile = await GetOrCreateProfileAsync(request.PersonId, ct);

        profile.RegisterInteraction(
            request.Kind,
            request.Reason);

        await repository.SaveChangesAsync(ct);

        return Map(profile);
    }

    public async Task<RelationshipProfileDto> AdjustAsync(
        AdjustRelationshipRequest request,
        CancellationToken ct)
    {
        var profile = await GetOrCreateProfileAsync(request.PersonId, ct);

        profile.Adjust(
            request.TrustDelta,
            request.WarmthDelta,
            request.RespectDelta,
            request.FamiliarityDelta,
            request.AnnoyanceDelta,
            request.OffenseDelta);

        await repository.SaveChangesAsync(ct);

        return Map(profile);
    }

    private async Task<RelationshipProfile> GetOrCreateProfileAsync(
        Guid personId,
        CancellationToken ct)
    {
        var profile = await repository.GetByPersonIdAsync(personId, ct);

        if (profile is not null)
            return profile;

        profile = RelationshipProfile.Create(personId);

        await repository.AddAsync(profile, ct);

        return profile;
    }

    private static RelationshipProfileDto Map(RelationshipProfile profile)
    {
        return new RelationshipProfileDto(
            profile.PersonId,
            profile.Trust,
            profile.Warmth,
            profile.Respect,
            profile.Familiarity,
            profile.Annoyance,
            profile.OffenseScore,
            profile.AccessLevel);
    }
}