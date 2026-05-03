namespace Nova.Modules.HomeAssistant.Contracts;

public sealed record HomeAssistantEntityDto(
    string EntityId,
    string State,
    string? FriendlyName,
    IReadOnlyDictionary<string, object?> Attributes);