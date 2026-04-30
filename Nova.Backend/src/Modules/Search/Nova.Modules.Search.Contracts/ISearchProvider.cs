namespace Nova.Modules.Search.Contracts;

public interface ISearchProvider
{
    Task<SearchResult> SearchAsync(
        SearchRequest request,
        CancellationToken ct);
}