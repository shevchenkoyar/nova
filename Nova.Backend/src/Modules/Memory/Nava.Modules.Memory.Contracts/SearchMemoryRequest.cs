namespace Nava.Modules.Memory.Contracts;

public sealed record SearchMemoryRequest(
    Guid UserId,
    string Query,
    int Limit = 10);