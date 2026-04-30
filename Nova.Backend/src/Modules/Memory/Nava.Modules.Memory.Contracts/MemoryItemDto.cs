namespace Nava.Modules.Memory.Contracts;

public sealed record MemoryItemDto(
    Guid Id,
    string Content,
    double Relevance,
    MemoryImportance Importance);