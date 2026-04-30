namespace Nova.Contracts.Conversation;

public sealed record ConversationMessageDto(
    Guid Id,
    Guid UserId,
    string Role,
    string Content,
    string? MetadataJson,
    DateTimeOffset CreatedAt);