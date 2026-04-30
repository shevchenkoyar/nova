using Nava.Modules.Memory.Contracts;
using Nava.Modules.Memory.Domain;

namespace Nava.Modules.Memory.Application;

public sealed class MemoryModuleApi(IMemoryRepository repository) : IMemoryModuleApi
{
    public async Task SaveFactAsync(
        SaveMemoryFactRequest request,
        CancellationToken ct)
    {
        var fact = MemoryFact.Create(
            request.UserId,
            request.Content,
            request.Source,
            (int)request.Importance);

        await repository.AddAsync(fact, ct);
    }

    public async Task<IReadOnlyList<MemoryItemDto>> SearchAsync(
        SearchMemoryRequest request,
        CancellationToken ct)
    {
        var facts = await repository.SearchAsync(
            request.UserId,
            request.Query,
            request.Limit,
            ct);

        return facts
            .Select(x => new MemoryItemDto(
                x.Id,
                x.Content,
                Relevance: 1.0,
                Importance: (MemoryImportance)x.Importance))
            .ToArray();
    }
}