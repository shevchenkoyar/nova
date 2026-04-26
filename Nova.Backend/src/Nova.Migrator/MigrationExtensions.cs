using Microsoft.EntityFrameworkCore;

namespace Nova.Migrator;

internal static partial class MigrationExtensions
{
    internal static async Task ApplyMigration<TDbContext>(this AsyncServiceScope scope, ILogger logger)
        where TDbContext : DbContext
    {
        await using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        logger.LogApplyingDatabaseMigrationForDbContext(typeof(TDbContext).Name);
        
        await context.Database.MigrateAsync();
    }

    [LoggerMessage(LogLevel.Information, "Applying database migration for {DbContext}")]
    static partial void LogApplyingDatabaseMigrationForDbContext(this ILogger logger, string dbContext);
}
