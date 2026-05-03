namespace Nova.Modules.HomeAssistant.Infrastructure;

public sealed class HomeAssistantOptions
{
    public const string SectionName = "HomeAssistant";

    public string BaseUrl { get; init; } = string.Empty;

    public string AccessToken { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 20;
}