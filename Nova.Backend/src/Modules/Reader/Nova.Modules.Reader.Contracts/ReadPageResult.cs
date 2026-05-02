namespace Nova.Modules.Reader.Contracts;

public sealed record ReadPageResult(
    string Url,
    string? Title,
    string Text,
    string? ContentType);