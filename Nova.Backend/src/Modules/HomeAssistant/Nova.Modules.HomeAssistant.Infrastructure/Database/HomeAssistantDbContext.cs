using Microsoft.EntityFrameworkCore;
using Nova.Modules.HomeAssistant.Domain;

namespace Nova.Modules.HomeAssistant.Infrastructure.Database;

public sealed class HomeAssistantDbContext(DbContextOptions<HomeAssistantDbContext> options) : DbContext(options)
{
    public DbSet<HomeAssistantEntity> Entities => Set<HomeAssistantEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("home_assistant");

        modelBuilder.Entity<HomeAssistantEntity>(builder =>
        {
            builder.ToTable("entities");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntityId)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasIndex(x => x.EntityId)
                .IsUnique();

            builder.Property(x => x.Domain)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.State)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.FriendlyName)
                .HasMaxLength(512);

            builder.Property(x => x.DeviceClass)
                .HasMaxLength(128);

            builder.Property(x => x.UnitOfMeasurement)
                .HasMaxLength(64);

            builder.Property(x => x.Area)
                .HasMaxLength(256);

            builder.Property(x => x.AttributesJson)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.SyncedAt)
                .IsRequired();

            builder.HasIndex(x => new { x.Domain, x.IsDeleted });
        });
    }
}