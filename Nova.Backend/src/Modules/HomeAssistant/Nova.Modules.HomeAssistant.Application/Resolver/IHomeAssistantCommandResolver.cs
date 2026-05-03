namespace Nova.Modules.HomeAssistant.Application.Resolver;

public interface IHomeAssistantCommandResolver
{
    Task<HomeAssistantCommandResolution> ResolveAsync(
        string command,
        CancellationToken ct);
}