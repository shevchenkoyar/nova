using Microsoft.EntityFrameworkCore;
using Nova.Modules.Memory.Domain;

namespace Nova.Modules.Memory.Infrastructure.Database;

public sealed class MemoryDbContext(DbContextOptions<MemoryDbContext> options) : DbContext(options)
{
    public DbSet<MemoryFact> Facts => Set<MemoryFact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.HasDefaultSchema("memory");

        modelBuilder.Entity<MemoryFact>(builder =>
        {
            builder.ToTable("facts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.Content)
                .HasMaxLength(8000)
                .IsRequired();

            builder.Property(x => x.Kind)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.Importance)
                .IsRequired();

            builder.Property(x => x.Source)
                .HasMaxLength(256);

            builder.Property(x => x.TagsJson)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(x => x.Embedding)
                .HasColumnType("vector(1536)");

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.LastAccessedAt);
            builder.Property(x => x.AccessCount).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Kind);
            builder.HasIndex(x => x.IsDeleted);
        });
    }
}