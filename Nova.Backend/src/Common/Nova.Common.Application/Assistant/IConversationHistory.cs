namespace Nova.Common.Application.Assistant;

public interface IConversationHistory
{
    Task AddUserMessageAsync(
        Guid userId,
        string text,
        CancellationToken ct);

    Task AddAssistantMessageAsync(
        Guid userId,
        string text,
        object? metadata,
        CancellationToken ct);

    Task AddPlannerMessageAsync(
        Guid userId,
        string text,
        object? metadata,
        CancellationToken ct);

    Task AddToolMessageAsync(
        Guid userId,
        string text,
        object? metadata,
        CancellationToken ct);
}