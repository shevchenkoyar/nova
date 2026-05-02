namespace Nova.Modules.Reader.Application.Summarization;

public sealed record PageSummaryResult(
    string Url,
    string? Title,
    string Summary,
    IReadOnlyList<string> KeyFacts);