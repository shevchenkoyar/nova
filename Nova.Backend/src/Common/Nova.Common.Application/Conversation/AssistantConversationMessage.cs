namespace Nova.Common.Application.Conversation;

public sealed record AssistantConversationMessage(
    string Role,
    string Content,
    DateTimeOffset CreatedAt);