namespace Nova.Modules.Search.Contracts;

public sealed record SearchResultItem(
    string Title,
    string Url,
    string Snippet);