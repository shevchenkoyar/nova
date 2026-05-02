namespace Nova.Modules.Reader.Infrastructure;

public sealed class ReaderInfrastructureOptions
{
    public const string SectionName = "Reader";

    public int TimeoutSeconds { get; init; } = 20;

    public int MaxResponseBytes { get; init; } = 2_000_000;

    public string UserAgent { get; init; } =
        "NovaReader/1.0 (+https://localhost)";
}