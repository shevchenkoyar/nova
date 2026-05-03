namespace Nova.Modules.HomeAssistant.Application.Resolver;

public sealed record ResolveHomeAssistantEntityRequest(
    string Device,
    string? Location,
    string? Domain);