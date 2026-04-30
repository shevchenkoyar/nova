using System.Text.Json.Serialization;

namespace Nova.Modules.Search.Infrastructure.Brave;

internal sealed class BraveWebResults
{
    [JsonPropertyName("results")]
    public IReadOnlyList<BraveWebResultItem> Results { get; init; } = [];
}