using System.Text.Json;
using Nova.Common.Application.Conversation;
using Nova.Modules.Conversation.Domain;

namespace Nova.Modules.Conversation.Application;

public sealed class ConversationHistoryService(
    IConversationMessageRepository repository)
    : IConversationHistory
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public Task AddUserMessageAsync(
        Guid userId,
        string text,
        CancellationToken ct)
    {
        return AddAsync(userId, ConversationMessageRole.User, text, null, ct);
    }

    public Task AddAssistantMessageAsync(
        Guid userId,
        string text,
        object? metadata,
        CancellationToken ct)
    {
        return AddAsync(userId, ConversationMessageRole.Assistant, text, metadata, ct);
    }

    public Task AddPlannerMessageAsync(
        Guid userId,
        string text,
        object? metadata,
        CancellationToken ct)
    {
        return AddAsync(userId, ConversationMessageRole.Planner, text, metadata, ct);
    }

    public Task AddToolMessageAsync(
        Guid userId,
        string text,
        object? metadata,
        CancellationToken ct)
    {
        return AddAsync(userId, ConversationMessageRole.Tool, text, metadata, ct);
    }

    public Task<IReadOnlyList<ConversationMessage>> GetRecentAsync(
        Guid userId,
        int limit,
        CancellationToken ct)
    {
        return repository.GetRecentAsync(userId, limit, ct);
    }

    private Task AddAsync(
        Guid userId,
        ConversationMessageRole role,
        string content,
        object? metadata,
        CancellationToken ct)
    {
        var metadataJson = metadata is null
            ? null
            : JsonSerializer.Serialize(metadata, JsonOptions);

        var message = ConversationMessage.Create(
            userId,
            role,
            content,
            metadataJson);

        return repository.AddAsync(message, ct);
    }
    
    public async Task<IReadOnlyList<AssistantConversationMessage>> GetRecentMessagesAsync(
        Guid userId,
        int limit,
        CancellationToken ct)
    {
        var messages = await repository.GetRecentAsync(
            userId,
            limit,
            ct);

        return messages
            .Where(x =>
                x.Role == ConversationMessageRole.User ||
                x.Role == ConversationMessageRole.Assistant)
            .Select(x => new AssistantConversationMessage(
                Role: x.Role == ConversationMessageRole.User ? "user" : "assistant",
                Content: x.Content,
                CreatedAt: x.CreatedAt))
            .ToArray();
    }
}