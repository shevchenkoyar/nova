using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Nova.Modules.Search.Contracts;

namespace Nova.Modules.Search.Infrastructure.Brave;

public sealed class BraveSearchProvider(
    HttpClient httpClient,
    IOptions<BraveSearchOptions> options)
    : ISearchProvider
{
    public async Task<SearchResult> SearchAsync(
        SearchRequest request,
        CancellationToken ct)
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            throw new InvalidOperationException(
                "Brave Search API key is not configured. Set Search:Brave:ApiKey.");
        }

        var limit = Math.Clamp(request.Limit, 1, 10);

        var url = $"/res/v1/web/search?q={Uri.EscapeDataString(request.Query)}&count={limit}";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

        httpRequest.Headers.Add("Accept", "application/json");
        httpRequest.Headers.Add("Accept-Encoding", "gzip");
        httpRequest.Headers.Add("X-Subscription-Token", settings.ApiKey);

        using var response = await httpClient.SendAsync(httpRequest, ct);

        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<BraveSearchResponse>(
            cancellationToken: ct);

        var items = dto?.Web?.Results
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.Title) &&
                !string.IsNullOrWhiteSpace(x.Url))
            .Select(x => new SearchResultItem(
                Title: x.Title!,
                Url: x.Url!,
                Snippet: x.Description ?? string.Empty))
            .ToArray() ?? [];

        return new SearchResult(
            Query: request.Query,
            Items: items);
    }
}