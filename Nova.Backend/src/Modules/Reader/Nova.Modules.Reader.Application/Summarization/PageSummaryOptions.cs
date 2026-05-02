namespace Nova.Modules.Reader.Application.Summarization;

public sealed record PageSummaryOptions(
    string Language = "ru",
    int MaxBullets = 7,
    bool IncludeKeyFacts = true);