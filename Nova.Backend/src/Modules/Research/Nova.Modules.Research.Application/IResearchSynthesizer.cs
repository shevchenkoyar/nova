using Nova.Modules.Research.Contracts;

namespace Nova.Modules.Research.Application;

public interface IResearchSynthesizer
{
    Task<string> SynthesizeAsync(
        string topic,
        IReadOnlyList<ResearchSourceSummary> sources,
        string language,
        CancellationToken ct);
}