using Nova.Common.Application.Tools;
using Nova.Modules.Search.Contracts;

namespace Nova.Modules.Search.Application.Tools;

public sealed class SearchWebTool(ISearchProvider searchProvider) : INovaTool
{
    public string Name => "search.web";

    public string Description => "Searches the web for information.";

    public string UsageRules => """
        Use this tool when the user asks to search, find, look up, research, or verify current information.

        Russian triggers:
        - "найди"
        - "поищи"
        - "проверь в интернете"
        - "узнай"
        - "найди информацию"
        - "погугли"

        English triggers:
        - "search"
        - "find"
        - "look up"
        - "research"
        - "verify"

        Always pass arguments as:
        { "query": "search query", "limit": 5 }

        If user asks in Russian, keep the query in Russian unless an English query is clearly better.
        Do not use this tool for ordinary conversation.
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
            },
            limit = new
            {
                type = "integer",
                description = "Maximum number of search results. Default is 5."
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
            return ToolResult.Failure("Search query is required.");

        var query = queryElement.GetString();

        if (string.IsNullOrWhiteSpace(query))
            return ToolResult.Failure("Search query is empty.");

        var limit = 5;

        if (context.Arguments.TryGetProperty("limit", out var limitElement)
            && limitElement.ValueKind == System.Text.Json.JsonValueKind.Number
            && limitElement.TryGetInt32(out var parsedLimit))
        {
            limit = Math.Clamp(parsedLimit, 1, 10);
        }

        var result = await searchProvider.SearchAsync(
            new SearchRequest(
                context.UserId,
                query.Trim(),
                limit),
            ct);

        if (result.Items.Count == 0)
        {
            return ToolResult.Success(
                "Я ничего не нашла по этому запросу.",
                result);
        }

        return ToolResult.Success(
            $"Нашла {result.Items.Count} результатов.",
            result);
    }
}