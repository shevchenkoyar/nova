namespace Nova.Modules.Search.Contracts;

public sealed record SearchResult(
    string Query,
    IReadOnlyList<SearchResultItem> Items);