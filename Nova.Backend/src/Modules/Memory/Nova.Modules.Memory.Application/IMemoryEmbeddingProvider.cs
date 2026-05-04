namespace Nova.Modules.Memory.Application;

public interface IMemoryEmbeddingProvider
{
    Task<float[]> EmbedAsync(
        string text,
        CancellationToken ct);
}