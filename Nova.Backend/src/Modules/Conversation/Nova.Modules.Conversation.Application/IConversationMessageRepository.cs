using Nova.Modules.Conversation.Domain;

namespace Nova.Modules.Conversation.Application;

public interface IConversationMessageRepository
{
    Task AddAsync(
        ConversationMessage message,
        CancellationToken ct);

    Task<IReadOnlyList<ConversationMessage>> GetRecentAsync(
        Guid userId,
        int limit,
        CancellationToken ct);
}