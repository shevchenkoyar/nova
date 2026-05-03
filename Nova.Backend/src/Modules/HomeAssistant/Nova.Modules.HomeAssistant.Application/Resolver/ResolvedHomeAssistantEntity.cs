namespace Nova.Modules.HomeAssistant.Application.Resolver;

public sealed record ResolvedHomeAssistantEntity(
    string EntityId,
    string Domain,
    string? FriendlyName,
    int Score);