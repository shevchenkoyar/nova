using System.Text.Json;
using OpenAI.Chat;

namespace Nova.Modules.HomeAssistant.Application.Resolver;

public sealed class OpenAiHomeAssistantCommandResolver(
    ChatClient client,
    IHomeAssistantEntityRepository repository)
    : IHomeAssistantCommandResolver
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<HomeAssistantCommandResolution> ResolveAsync(
        string command,
        CancellationToken ct)
    {
        var entities = await repository.GetActiveAsync(ct);

        var catalog = entities
            .Select(x => new HomeAssistantEntityCatalogItem(
                x.EntityId,
                x.Domain,
                x.FriendlyName,
                x.Area,
                x.DeviceClass,
                x.State))
            .ToArray();

        var catalogJson = JsonSerializer.Serialize(
            catalog,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        var response = await client.CompleteChatAsync(
            [
                new SystemChatMessage(BuildSystemPrompt(catalogJson)),
                new UserChatMessage(command)
            ],
            cancellationToken: ct);

        var content = response.Value.Content[0].Text;

        return Parse(content);
    }

    private static string BuildSystemPrompt(string catalogJson)
    {
        return $$"""
You are a Home Assistant command resolver.

Your task:
Convert a user's natural language smart home command into an exact Home Assistant service call.

Available Home Assistant entities:
{{catalogJson}}

Return ONLY valid JSON.

When resolved:
{
  "isResolved": true,
  "entityId": "light.toilet",
  "domain": "light",
  "service": "turn_on",
  "data": {},
  "reason": "short explanation"
}

When not resolved:
{
  "isResolved": false,
  "entityId": null,
  "domain": null,
  "service": null,
  "data": null,
  "reason": "why it cannot be resolved"
}

For state questions, resolve only entityId/domain.
Use service = "get_state".
Do not call Home Assistant service for state questions.

Rules:
- Use ONLY entityId values from the available entities list.
- Do NOT invent entity ids.
- Domain MUST match selected entity domain.
- For Russian "включи" use service "turn_on".
- For Russian "выключи" use service "turn_off".
- For Russian "переключи" use service "toggle".
- For English "turn on" use service "turn_on".
- For English "turn off" use service "turn_off".
- For English "toggle" use service "toggle".
- For light domain use: turn_on, turn_off, toggle.
- For switch domain use: turn_on, turn_off, toggle.
- For scene domain use: turn_on.
- For media_player domain use: turn_on, turn_off, toggle, media_play, media_pause, volume_set.
- If command asks to set volume, use media_player.volume_set and data: { "volume_level": 0.2 } for 20%.
- Prefer friendlyName and area when matching rooms and locations.
- If several entities match equally, return isResolved=false with ambiguity reason.
- If confidence is low, return isResolved=false.
- Return JSON only.
- No markdown.
""";
    }

    private static HomeAssistantCommandResolution Parse(string content)
    {
        try
        {
            content = content.Trim();

            var start = content.IndexOf('{');
            var end = content.LastIndexOf('}');

            if (start < 0 || end <= start)
            {
                return new HomeAssistantCommandResolution(
                    IsResolved: false,
                    EntityId: null,
                    Domain: null,
                    Service: null,
                    Data: null,
                    Reason: "Failed to parse resolver response.");
            }

            var json = content[start..(end + 1)];

            var dto = JsonSerializer.Deserialize<HomeAssistantCommandResolutionDto>(
                json,
                JsonOptions);

            if (dto is null)
            {
                return new HomeAssistantCommandResolution(
                    IsResolved: false,
                    EntityId: null,
                    Domain: null,
                    Service: null,
                    Data: null,
                    Reason: "Empty resolver response.");
            }

            return new HomeAssistantCommandResolution(
                IsResolved: dto.IsResolved,
                EntityId: dto.EntityId,
                Domain: dto.Domain,
                Service: dto.Service,
                Data: dto.Data,
                Reason: dto.Reason);
        }
        catch (Exception ex)
        {
            return new HomeAssistantCommandResolution(
                IsResolved: false,
                EntityId: null,
                Domain: null,
                Service: null,
                Data: null,
                Reason: ex.Message);
        }
    }

    private sealed record HomeAssistantEntityCatalogItem(
        string EntityId,
        string Domain,
        string? FriendlyName,
        string? Area,
        string? DeviceClass,
        string State);

    private sealed record HomeAssistantCommandResolutionDto(
        bool IsResolved,
        string? EntityId,
        string? Domain,
        string? Service,
        Dictionary<string, object?>? Data,
        string? Reason);
}