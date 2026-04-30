using Microsoft.EntityFrameworkCore;
using Nova.Modules.Conversation.Domain;

namespace Nova.Modules.Conversation.Infrastructure.Database;

public sealed class ConversationDbContext(
    DbContextOptions<ConversationDbContext> options)
    : DbContext(options)
{
    public DbSet<ConversationMessage> Messages => Set<ConversationMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("conversation");

        modelBuilder.Entity<ConversationMessage>(builder =>
        {
            builder.ToTable("messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Role)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Content)
                .HasMaxLength(32_000)
                .IsRequired();

            builder.Property(x => x.MetadataJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.CreatedAt });
        });
    }
}