namespace Nova.Modules.HomeAssistant.Contracts;

public sealed record CallHomeAssistantServiceRequest(
    Guid UserId,
    string Domain,
    string Service,
    string EntityId,
    IReadOnlyDictionary<string, object?>? Data = null);