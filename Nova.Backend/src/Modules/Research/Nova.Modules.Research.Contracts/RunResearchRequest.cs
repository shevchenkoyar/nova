namespace Nova.Modules.Research.Contracts;

public sealed record RunResearchRequest(
    Guid UserId,
    string Topic,
    ResearchDepth Depth = ResearchDepth.Standard,
    string Language = "ru",
    int MaxSources = 5);