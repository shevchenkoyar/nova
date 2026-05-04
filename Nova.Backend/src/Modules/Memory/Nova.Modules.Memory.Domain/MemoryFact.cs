using Pgvector;

namespace Nova.Modules.Memory.Domain;

public sealed class MemoryFact
{
    private MemoryFact()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public string Kind { get; private set; } = "fact";

    public int Importance { get; private set; }

    public string? Source { get; private set; }

    public string TagsJson { get; private set; } = "[]";

    public Vector? Embedding { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? LastAccessedAt { get; private set; }

    public int AccessCount { get; private set; }

    public bool IsDeleted { get; private set; }

    public static MemoryFact Create(
        Guid userId,
        string content,
        string kind,
        int importance,
        string? source,
        string tagsJson,
        Vector? embedding)
    {
        var now = DateTimeOffset.UtcNow;

        return new MemoryFact
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = content.Trim(),
            Kind = kind,
            Importance = Math.Clamp(importance, 1, 100),
            Source = source,
            TagsJson = tagsJson,
            Embedding = embedding,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };
    }

    public void Touch()
    {
        LastAccessedAt = DateTimeOffset.UtcNow;
        AccessCount++;
    }

    public void UpdateEmbedding(Vector embedding)
    {
        Embedding = embedding;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}