using Nova.Modules.Search.Contracts;

namespace Nova.Modules.Search.Infrastructure;

public sealed class FakeSearchProvider : ISearchProvider
{
    public Task<SearchResult> SearchAsync(
        SearchRequest request,
        CancellationToken ct)
    {
        var result = new SearchResult(
            request.Query,
            [
                new SearchResultItem(
                    "Fake result",
                    "https://example.com",
                    $"Результат поиска по запросу: {request.Query}")
            ]);

        return Task.FromResult(result);
    }
}