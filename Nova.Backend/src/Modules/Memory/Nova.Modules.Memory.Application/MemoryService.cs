using System.Text.Json;
using Nova.Modules.Memory.Domain;
using Pgvector;

namespace Nova.Modules.Memory.Application;

public sealed class MemoryService(
    IMemoryRepository repository,
    IMemoryEmbeddingProvider embeddingProvider)
{
    public async Task SaveFactAsync(
        Guid userId,
        string content,
        string kind,
        int importance,
        string? source,
        IReadOnlyList<string> tags,
        CancellationToken ct)
    {
        var embedding = await embeddingProvider.EmbedAsync(content, ct);

        var fact = MemoryFact.Create(
            userId,
            content,
            kind,
            importance,
            source,
            JsonSerializer.Serialize(tags),
            new Vector(embedding));

        await repository.AddAsync(fact, ct);
        await repository.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<MemoryFact>> SearchAsync(
        Guid userId,
        string query,
        int limit,
        CancellationToken ct)
    {
        var embedding = await embeddingProvider.EmbedAsync(query, ct);

        return await repository.SearchAsync(
            userId,
            query,
            embedding,
            limit,
            ct);
    }
}