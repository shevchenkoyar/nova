namespace Nava.Modules.Memory.Contracts;

public interface IMemoryModuleApi
{
    Task SaveFactAsync(
        SaveMemoryFactRequest request,
        CancellationToken ct);

    Task<IReadOnlyList<MemoryItemDto>> SearchAsync(
        SearchMemoryRequest request,
        CancellationToken ct);
}