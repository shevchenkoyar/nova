namespace Nova.Modules.HomeAssistant.Contracts;

public interface IHomeAssistantClient
{
    Task<IReadOnlyList<HomeAssistantEntityDto>> GetStatesAsync(
        CancellationToken ct);

    Task<HomeAssistantEntityDto?> GetStateAsync(
        string entityId,
        CancellationToken ct);

    Task CallServiceAsync(
        CallHomeAssistantServiceRequest request,
        CancellationToken ct);
}