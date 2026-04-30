using Nava.Modules.Memory.Contracts;
using Nova.Common.Application.Tools;

namespace Nava.Modules.Memory.Application.Tools;

public sealed class SearchMemoryTool(IMemoryModuleApi memory) : INovaTool
{
    public string Name => "memory.search";

    public string Description =>
        "Searches user's long-term memory.";

    public string UsageRules => """
                                Use this tool when the user asks what you remember, asks about saved preferences,
                                or when a plan needs to retrieve relevant long-term memory.
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
                description = "Memory search query."
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

        var result = await memory.SearchAsync(
            new SearchMemoryRequest(context.UserId, query),
            ct);

        return ToolResult.Success(
            result.Count == 0 ? "Ничего не нашла в памяти." : "Нашла в памяти.",
            result);
    }
}