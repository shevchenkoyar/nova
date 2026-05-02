namespace Nova.Modules.Research.Contracts;

public sealed record ResearchResult(
    string Topic,
    string Summary,
    IReadOnlyList<ResearchSourceSummary> Sources);