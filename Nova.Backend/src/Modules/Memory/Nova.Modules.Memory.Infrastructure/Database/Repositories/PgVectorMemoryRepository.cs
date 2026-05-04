using Microsoft.EntityFrameworkCore;
using Nova.Modules.Memory.Application;
using Nova.Modules.Memory.Domain;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Nova.Modules.Memory.Infrastructure.Database.Repositories;


public sealed class PgVectorMemoryRepository(MemoryDbContext dbContext) : IMemoryRepository
{
    public async Task AddAsync(
        MemoryFact fact,
        CancellationToken ct)
    {
        await dbContext.Facts.AddAsync(fact, ct);
    }

    public async Task<IReadOnlyList<MemoryFact>> SearchAsync(
        Guid userId,
        string query,
        float[] embedding,
        int limit,
        CancellationToken ct)
    {
        var queryVector = new Vector(embedding);
        var tokens = Tokenize(query);

        var candidates = await dbContext.Facts
            .Where(x => x.UserId == userId)
            .Where(x => !x.IsDeleted)
            .Where(x => x.Embedding != null)
            .Select(x => new
            {
                Fact = x,
                Distance = x.Embedding!.CosineDistance(queryVector)
            })
            .OrderBy(x => x.Distance)
            .Take(Math.Max(limit * 5, 30))
            .ToListAsync(ct);

        var ranked = candidates
            .Select(x => new
            {
                x.Fact,
                Score =
                    VectorScore(x.Distance) +
                    KeywordScore(x.Fact.Content, tokens) +
                    ImportanceScore(x.Fact.Importance) +
                    RecencyScore(x.Fact.CreatedAt)
            })
            .OrderByDescending(x => x.Score)
            .Take(limit)
            .Select(x => x.Fact)
            .ToArray();

        foreach (var fact in ranked)
        {
            fact.Touch();
        }

        await dbContext.SaveChangesAsync(ct);

        return ranked;
    }

    private static double VectorScore(double distance)
    {
        return Math.Max(0, 1 - distance) * 100;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return dbContext.SaveChangesAsync(ct);
    }

    private static double KeywordScore(string content, IReadOnlyList<string> tokens)
    {
        var normalized = Normalize(content);
        var score = 0d;

        foreach (var token in tokens)
        {
            if (normalized.Contains(token))
                score += 8;
        }

        return score;
    }

    private static double ImportanceScore(int importance)
    {
        return importance * 0.3;
    }

    private static double RecencyScore(DateTimeOffset createdAt)
    {
        var days = (DateTimeOffset.UtcNow - createdAt).TotalDays;

        return days switch
        {
            < 1 => 10,
            < 7 => 6,
            < 30 => 3,
            _ => 0
        };
    }

    private static string Normalize(string text)
    {
        return text
            .Trim()
            .ToLowerInvariant()
            .Replace("ё", "е")
            .Replace(".", " ")
            .Replace(",", " ")
            .Replace(":", " ")
            .Replace(";", " ")
            .Replace("!", " ")
            .Replace("?", " ")
            .Replace("-", " ")
            .Replace("_", " ");
    }

    private static IReadOnlyList<string> Tokenize(string text)
    {
        return Normalize(text)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => x.Length >= 3)
            .Distinct()
            .ToArray();
    }
}