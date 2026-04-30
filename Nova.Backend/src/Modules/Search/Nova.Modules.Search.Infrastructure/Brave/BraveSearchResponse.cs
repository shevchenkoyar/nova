using System.Text.Json.Serialization;

namespace Nova.Modules.Search.Infrastructure.Brave;

internal sealed class BraveSearchResponse
{
    [JsonPropertyName("web")]
    public BraveWebResults? Web { get; init; }
}