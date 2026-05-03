using Microsoft.EntityFrameworkCore;
using Nova.Modules.HomeAssistant.Application;
using Nova.Modules.HomeAssistant.Domain;

namespace Nova.Modules.HomeAssistant.Infrastructure.Database.Repositories;

public sealed class HomeAssistantEntityRepository(HomeAssistantDbContext dbContext) : IHomeAssistantEntityRepository
{
    public async Task<IReadOnlyList<HomeAssistantEntity>> GetAllAsync(
        CancellationToken ct)
    {
        return await dbContext.Entities
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<HomeAssistantEntity>> GetActiveAsync(
        CancellationToken ct)
    {
        return await dbContext.Entities
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
    }

    public Task<HomeAssistantEntity?> GetByEntityIdAsync(
        string entityId,
        CancellationToken ct)
    {
        return dbContext.Entities
            .FirstOrDefaultAsync(x => x.EntityId == entityId, ct);
    }

    public async Task AddAsync(
        HomeAssistantEntity entity,
        CancellationToken ct)
    {
        await dbContext.Entities.AddAsync(entity, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}