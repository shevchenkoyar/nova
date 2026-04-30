using Nova.Common.Application.Tools;

namespace Nova.Modules.Search.Application;

public sealed class SearchWebTool : INovaTool
{
    public string Name => "search.web";

    public string Description => "Searches the web for information.";

    public string UsageRules => """
                                Use this tool when the user asks to search, find, look up, research, or verify current information.
                                Russian triggers: "найди", "поищи", "проверь в интернете", "узнай".
                                Always pass arguments as:
                                { "query": "search query" }
                                """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            query = new
            {
                type = "string",
                description = "The search query."
            }
        },
        required = new[] { "query" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.SafeAction;

    public Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("query", out var queryElement))
            return Task.FromResult(ToolResult.Failure("Search query is required."));

        var query = queryElement.GetString();

        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult(ToolResult.Failure("Search query is empty."));

        var result = $"Найдено по запросу: {query}";

        return Task.FromResult(ToolResult.Success(result));
    }
}