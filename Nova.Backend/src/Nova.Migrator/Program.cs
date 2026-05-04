using Nova.Migrator;
using Nova.Modules.Conversation.Infrastructure;
using Nova.Modules.Conversation.Infrastructure.Database;
using Nova.Modules.HomeAssistant.Infrastructure;
using Nova.Modules.HomeAssistant.Infrastructure.Database;
using Nova.Modules.Memory.Infrastructure;
using Nova.Modules.Memory.Infrastructure.Database;
using Nova.Modules.Relationships.Infrastructure;
using Nova.Modules.Relationships.Infrastructure.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRelationshipsModule(builder.Configuration);
builder.Services.AddConversationModule(builder.Configuration);
builder.Services.AddHomeAssistantModuleDatabase(builder.Configuration);
builder.Services.AddMemoryModule(builder.Configuration);

using var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();

var logger = scope.ServiceProvider
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Nova.Migrator");

logger.LogInformation("Applying database migrations...");

await scope.ApplyMigration<RelationshipsDbContext>(logger);
await scope.ApplyMigration<ConversationDbContext>(logger);
await scope.ApplyMigration<HomeAssistantDbContext>(logger);
await scope.ApplyMigration<MemoryDbContext>(logger);

logger.LogInformation("Database migrations applied successfully.");