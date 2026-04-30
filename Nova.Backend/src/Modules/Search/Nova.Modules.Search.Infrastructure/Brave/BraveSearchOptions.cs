namespace Nova.Modules.Search.Infrastructure.Brave;

public sealed class BraveSearchOptions
{
    public const string SectionName = "Search:Brave";

    public string BaseUrl { get; init; } = "https://api.search.brave.com";

    public string ApiKey { get; init; } = string.Empty;
}