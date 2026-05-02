using Nova.Modules.Reader.Contracts;

namespace Nova.Modules.Reader.Application.Summarization;

public interface IPageSummarizer
{
    Task<PageSummaryResult> SummarizeAsync(
        ReadPageResult page,
        PageSummaryOptions options,
        CancellationToken ct);
}