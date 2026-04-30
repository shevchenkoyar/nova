using Nova.Modules.Relationships.Domain;

namespace Nova.Modules.Relationships.Application;

public interface IRelationshipProfileRepository
{
    Task<RelationshipProfile?> GetByPersonIdAsync(
        Guid personId,
        CancellationToken ct);

    Task AddAsync(
        RelationshipProfile profile,
        CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}