var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// builder.Services.AddInfrastructure(builder.Configuration);

using var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();

var logger = scope.ServiceProvider
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Nova.Migrator");

logger.LogInformation("Applying database migrations...");

// scope.ApplyMigration<ConversationDbContext>(logger);
// scope.ApplyMigration<MemoryDbContext>(logger);

logger.LogInformation("Database migrations applied successfully.");