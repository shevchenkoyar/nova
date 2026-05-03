namespace Nova.Modules.HomeAssistant.Application.Resolver;

public interface IHomeAssistantEntityResolver
{
    Task<ResolvedHomeAssistantEntity?> ResolveAsync(
        ResolveHomeAssistantEntityRequest request,
        CancellationToken ct);
}