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
        var profile = await repository.GetByPersonIdAsync(personId, ct);

        if (profile is null)
        {
            profile = RelationshipProfile.Create(personId);

            await repository.AddAsync(profile, ct);
            await repository.SaveChangesAsync(ct);
        }

        return Map(profile);
    }

    public async Task<RelationshipProfileDto> RegisterInteractionAsync(
        RegisterInteractionRequest request,
        CancellationToken ct)
    {
        var profile = await repository.GetByPersonIdAsync(request.PersonId, ct);

        if (profile is null)
        {
            profile = RelationshipProfile.Create(request.PersonId);
            await repository.AddAsync(profile, ct);
        }

        profile.RegisterInteraction(request.Kind, request.Reason);

        await repository.SaveChangesAsync(ct);

        return Map(profile);
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