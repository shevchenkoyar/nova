namespace Nova.Modules.Search.Contracts;

public sealed record SearchRequest(
    Guid UserId,
    string Query,
    int Limit = 5);