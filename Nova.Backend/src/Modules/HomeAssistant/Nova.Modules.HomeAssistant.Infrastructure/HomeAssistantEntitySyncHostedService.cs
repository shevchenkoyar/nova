using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nova.Modules.HomeAssistant.Application;

namespace Nova.Modules.HomeAssistant.Infrastructure;

public sealed class HomeAssistantEntitySyncHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<HomeAssistantEntitySyncHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var sync = scope.ServiceProvider
                .GetRequiredService<HomeAssistantEntitySyncService>();
            await sync.SyncAsync(stoppingToken);
            logger.LogInformation("Home Assistant entities synchronized.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to synchronize Home Assistant entities.");
        }
    }
}