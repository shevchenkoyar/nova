namespace Nava.Modules.Memory.Contracts;

public sealed record SaveMemoryFactRequest(
    Guid UserId,
    string Content,
    string Source,
    MemoryImportance Importance);