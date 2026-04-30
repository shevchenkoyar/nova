namespace Nava.Modules.Memory.Domain;

public sealed class MemoryFact
{
    private MemoryFact()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

    public int Importance { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static MemoryFact Create(
        Guid userId,
        string content,
        string source,
        int importance)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        return new MemoryFact
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = content.Trim(),
            Source = source.Trim(),
            Importance = importance,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}