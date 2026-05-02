namespace Nova.Modules.Research.Contracts;

public sealed record ResearchSourceSummary(
    string Url,
    string? Title,
    string Summary);