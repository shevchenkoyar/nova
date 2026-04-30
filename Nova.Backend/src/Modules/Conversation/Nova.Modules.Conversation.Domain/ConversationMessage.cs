namespace Nova.Modules.Conversation.Domain;

public sealed class ConversationMessage
{
    private ConversationMessage()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public ConversationMessageRole Role { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public string? MetadataJson { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static ConversationMessage Create(
        Guid userId,
        ConversationMessageRole role,
        string content,
        string? metadataJson = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        return new ConversationMessage
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Role = role,
            Content = content.Trim(),
            MetadataJson = metadataJson,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}