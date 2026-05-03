using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nova.Modules.HomeAssistant.Contracts;

namespace Nova.Modules.HomeAssistant.Infrastructure;

public sealed class HomeAssistantRestClient(HttpClient httpClient) : IHomeAssistantClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<HomeAssistantEntityDto>> GetStatesAsync(
        CancellationToken ct)
    {
        var states = await httpClient.GetFromJsonAsync<List<HomeAssistantStateDto>>(
            "/api/states",
            JsonOptions,
            ct);

        return states?
            .Select(Map)
            .ToArray() ?? [];
    }

    public async Task<HomeAssistantEntityDto?> GetStateAsync(
        string entityId,
        CancellationToken ct)
    {
        using var response = await httpClient.GetAsync(
            $"/api/states/{Uri.EscapeDataString(entityId)}",
            ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<HomeAssistantStateDto>(
            JsonOptions,
            ct);

        return dto is null ? null : Map(dto);
    }

    public async Task CallServiceAsync(
        CallHomeAssistantServiceRequest request,
        CancellationToken ct)
    {
        var data = new Dictionary<string, object?>
        {
            ["entity_id"] = request.EntityId
        };

        if (request.Data is not null)
        {
            foreach (var pair in request.Data)
            {
                data[pair.Key] = pair.Value;
            }
        }

        using var response = await httpClient.PostAsJsonAsync(
            $"/api/services/{request.Domain}/{request.Service}",
            data,
            JsonOptions,
            ct);

        response.EnsureSuccessStatusCode();
    }

    private static HomeAssistantEntityDto Map(HomeAssistantStateDto dto)
    {
        var attributes = dto.Attributes?
            .ToDictionary(
                x => x.Key,
                x => ConvertJsonElement(x.Value))
            ?? new Dictionary<string, object?>();

        var friendlyName = attributes.TryGetValue("friendly_name", out var friendlyValue)
            ? friendlyValue?.ToString()
            : null;

        return new HomeAssistantEntityDto(
            EntityId: dto.EntityId,
            State: dto.State,
            FriendlyName: friendlyName,
            Attributes: attributes);
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64(out var longValue) => longValue,
            JsonValueKind.Number when element.TryGetDouble(out var doubleValue) => doubleValue,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.GetRawText()
        };
    }

    private sealed class HomeAssistantStateDto
    {
        [JsonPropertyName("entity_id")]
        public string EntityId { get; init; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; init; } = string.Empty;

        [JsonPropertyName("attributes")]
        public Dictionary<string, JsonElement>? Attributes { get; init; }
    }
}