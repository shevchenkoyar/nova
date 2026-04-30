using Microsoft.EntityFrameworkCore;
using Nova.Modules.Relationships.Domain;

namespace Nova.Modules.Relationships.Infrastructure.Database;

public sealed class RelationshipsDbContext(
    DbContextOptions<RelationshipsDbContext> options)
    : DbContext(options)
{
    public DbSet<RelationshipProfile> Profiles => Set<RelationshipProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("relationships");

        modelBuilder.Entity<RelationshipProfile>(builder =>
        {
            builder.ToTable("profiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PersonId)
                .IsRequired();

            builder.HasIndex(x => x.PersonId)
                .IsUnique();

            builder.Property(x => x.Trust)
                .IsRequired();

            builder.Property(x => x.Warmth)
                .IsRequired();

            builder.Property(x => x.Respect)
                .IsRequired();

            builder.Property(x => x.Familiarity)
                .IsRequired();

            builder.Property(x => x.Annoyance)
                .IsRequired();

            builder.Property(x => x.OffenseScore)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Ignore(x => x.AccessLevel);
        });
    }
}