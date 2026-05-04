using Nova.Modules.Memory.Application;
using OpenAI.Embeddings;

namespace Nova.Modules.Memory.Infrastructure;

public sealed class OpenAiMemoryEmbeddingProvider(EmbeddingClient client) : IMemoryEmbeddingProvider
{
    public async Task<float[]> EmbedAsync(
        string text,
        CancellationToken ct)
    {
        var response = await client.GenerateEmbeddingAsync(
            text,
            cancellationToken: ct);

        return response.Value.ToFloats().ToArray();
    }
}