using Nova.Modules.HomeAssistant.Domain;

namespace Nova.Modules.HomeAssistant.Application;

public interface IHomeAssistantEntityRepository
{
    Task<IReadOnlyList<HomeAssistantEntity>> GetAllAsync(
        CancellationToken ct);

    Task<IReadOnlyList<HomeAssistantEntity>> GetActiveAsync(
        CancellationToken ct);

    Task<HomeAssistantEntity?> GetByEntityIdAsync(
        string entityId,
        CancellationToken ct);

    Task AddAsync(
        HomeAssistantEntity entity,
        CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}