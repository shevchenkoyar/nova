using Nova.Modules.Reader.Application.Summarization;
using Nova.Modules.Reader.Contracts;
using Nova.Modules.Research.Contracts;
using Nova.Modules.Search.Contracts;

namespace Nova.Modules.Research.Application;

public sealed class ResearchService(
    ISearchProvider searchProvider,
    IPageReader pageReader,
    IPageSummarizer pageSummarizer,
    IResearchSynthesizer researchSynthesizer)
    : IResearchService
{
    public async Task<ResearchResult> RunAsync(
        RunResearchRequest request,
        CancellationToken ct)
    {
        var maxSources = request.Depth switch
        {
            ResearchDepth.Quick => Math.Clamp(request.MaxSources, 1, 3),
            ResearchDepth.Standard => Math.Clamp(request.MaxSources, 3, 6),
            ResearchDepth.Deep => Math.Clamp(request.MaxSources, 5, 10),
            _ => Math.Clamp(request.MaxSources, 3, 6)
        };

        var searchResult = await searchProvider.SearchAsync(
            new SearchRequest(
                request.UserId,
                request.Topic,
                maxSources),
            ct);

        var sourceSummaries = new List<ResearchSourceSummary>();

        foreach (var item in searchResult.Items.Take(maxSources))
        {
            try
            {
                var page = await pageReader.FetchAsync(
                    new FetchUrlRequest(
                        request.UserId,
                        item.Url,
                        MaxTextLength: 30_000),
                    ct);

                var summary = await pageSummarizer.SummarizeAsync(
                    page,
                    new PageSummaryOptions(
                        Language: request.Language,
                        MaxBullets: 5),
                    ct);

                sourceSummaries.Add(new ResearchSourceSummary(
                    Url: item.Url,
                    Title: page.Title ?? item.Title,
                    Summary: summary.Summary));
            }
            catch
            {
                // Пока пропускаем недоступные страницы.
                // Позже добавим source failures в результат.
            }
        }

        var finalSummary = await researchSynthesizer.SynthesizeAsync(
            request.Topic,
            sourceSummaries,
            request.Language,
            ct);

        return new ResearchResult(
            Topic: request.Topic,
            Summary: finalSummary,
            Sources: sourceSummaries);
    }
}