using System.Text.Json;
using Nova.Modules.HomeAssistant.Contracts;
using Nova.Modules.HomeAssistant.Domain;

namespace Nova.Modules.HomeAssistant.Application;

public sealed class HomeAssistantEntitySyncService(
    IHomeAssistantClient client,
    IHomeAssistantEntityRepository repository)
{
    public async Task SyncAsync(CancellationToken ct)
    {
        var remoteEntities = await client.GetStatesAsync(ct);
        var localEntities = await repository.GetAllAsync(ct);

        var remoteById = remoteEntities.ToDictionary(x => x.EntityId);
        var localById = localEntities.ToDictionary(x => x.EntityId);

        foreach (var remote in remoteEntities)
        {
            var attributesJson = JsonSerializer.Serialize(remote.Attributes);

            remote.Attributes.TryGetValue("device_class", out var deviceClass);
            remote.Attributes.TryGetValue("unit_of_measurement", out var unit);
            remote.Attributes.TryGetValue("area", out var area);

            if (localById.TryGetValue(remote.EntityId, out var local))
            {
                local.Update(
                    state: remote.State,
                    friendlyName: remote.FriendlyName,
                    deviceClass: deviceClass?.ToString(),
                    unitOfMeasurement: unit?.ToString(),
                    area: area?.ToString(),
                    attributesJson: attributesJson);
            }
            else
            {
                var entity = HomeAssistantEntity.Create(
                    entityId: remote.EntityId,
                    state: remote.State,
                    friendlyName: remote.FriendlyName,
                    deviceClass: deviceClass?.ToString(),
                    unitOfMeasurement: unit?.ToString(),
                    area: area?.ToString(),
                    attributesJson: attributesJson);

                await repository.AddAsync(entity, ct);
            }
        }

        foreach (var local in localEntities)
        {
            if (!remoteById.ContainsKey(local.EntityId))
            {
                local.MarkDeleted();
            }
        }

        await repository.SaveChangesAsync(ct);
    }
}