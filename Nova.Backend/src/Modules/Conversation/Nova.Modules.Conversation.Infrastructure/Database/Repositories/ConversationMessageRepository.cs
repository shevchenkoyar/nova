using Microsoft.EntityFrameworkCore;
using Nova.Modules.Conversation.Application;
using Nova.Modules.Conversation.Domain;

namespace Nova.Modules.Conversation.Infrastructure.Database.Repositories;

public sealed class ConversationMessageRepository(ConversationDbContext dbContext) : IConversationMessageRepository
{
    public async Task AddAsync(
        ConversationMessage message,
        CancellationToken ct)
    {
        await dbContext.Messages.AddAsync(message, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ConversationMessage>> GetRecentAsync(
        Guid userId,
        int limit,
        CancellationToken ct)
    {
        return await dbContext.Messages
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);
    }
}