using System.Text.Json.Serialization;

namespace Nova.Modules.Search.Infrastructure.Brave;

internal sealed class BraveWebResultItem
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}