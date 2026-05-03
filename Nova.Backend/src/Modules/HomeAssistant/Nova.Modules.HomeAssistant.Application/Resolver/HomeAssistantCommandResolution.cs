namespace Nova.Modules.HomeAssistant.Application.Resolver;

public sealed record HomeAssistantCommandResolution(
    bool IsResolved,
    string? EntityId,
    string? Domain,
    string? Service,
    Dictionary<string, object?>? Data,
    string? Reason);