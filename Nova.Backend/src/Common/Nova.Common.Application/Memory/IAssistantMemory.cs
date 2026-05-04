namespace Nova.Common.Application.Memory;

public interface IAssistantMemory
{
    Task<IReadOnlyList<AssistantMemoryItem>> SearchAsync(
        Guid userId,
        string query,
        int limit,
        CancellationToken ct);
}