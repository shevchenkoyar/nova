namespace Nova.Modules.Research.Contracts;

public interface IResearchService
{
    Task<ResearchResult> RunAsync(
        RunResearchRequest request,
        CancellationToken ct);
}