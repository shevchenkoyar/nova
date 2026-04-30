using Microsoft.EntityFrameworkCore;
using Nova.Modules.Relationships.Application;
using Nova.Modules.Relationships.Domain;

namespace Nova.Modules.Relationships.Infrastructure.Database.Repositories;

public sealed class RelationshipProfileRepository(RelationshipsDbContext dbContext) : IRelationshipProfileRepository
{
    public Task<RelationshipProfile?> GetByPersonIdAsync(
        Guid personId,
        CancellationToken ct)
    {
        return dbContext.Profiles
            .FirstOrDefaultAsync(x => x.PersonId == personId, ct);
    }

    public async Task AddAsync(
        RelationshipProfile profile,
        CancellationToken ct)
    {
        await dbContext.Profiles.AddAsync(profile, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}