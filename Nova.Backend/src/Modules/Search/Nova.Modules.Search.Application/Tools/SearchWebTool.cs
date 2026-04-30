using Nova.Common.Application.Tools;
using Nova.Modules.Search.Contracts;

namespace Nova.Modules.Search.Application.Tools;

public sealed class SearchWebTool(ISearchProvider searchProvider) : INovaTool
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

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.ReadOnly;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("query", out var queryElement))
            return ToolResult.Failure("Query is required.");

        var query = queryElement.GetString();

        if (string.IsNullOrWhiteSpace(query))
            return ToolResult.Failure("Query is empty.");

        var result = await searchProvider.SearchAsync(
            new SearchRequest(context.UserId, query),
            ct);

        return ToolResult.Success(
            $"Нашла {result.Items.Count} результатов.",
            result);
    }
}