namespace Nova.Modules.Reader.Contracts;

public sealed record FetchUrlRequest(
    Guid UserId,
    string Url,
    int MaxTextLength = 20_000);