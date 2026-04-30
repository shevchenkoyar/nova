using Nova.Migrator;
using Nova.Modules.Conversation.Infrastructure;
using Nova.Modules.Conversation.Infrastructure.Database;
using Nova.Modules.Relationships.Infrastructure;
using Nova.Modules.Relationships.Infrastructure.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRelationshipsModule(builder.Configuration);
builder.Services.AddConversationModule(builder.Configuration);

using var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();

var logger = scope.ServiceProvider
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Nova.Migrator");

logger.LogInformation("Applying database migrations...");

await scope.ApplyMigration<RelationshipsDbContext>(logger);
await scope.ApplyMigration<ConversationDbContext>(logger);

logger.LogInformation("Database migrations applied successfully.");