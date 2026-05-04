using System.Text.Json;
using Nova.Common.Application.Tools;

namespace Nova.Modules.Memory.Application.Tools;

public sealed class SearchMemoryTool(
    MemoryService memory)
    : INovaTool
{
    public string Name => "memory.search";

    public string Description =>
        "Searches user's long-term memory using semantic and keyword search.";

    public string UsageRules => """
Use this tool when:
- user asks what you remember
- user asks about preferences or past facts
- you need additional context about the user

Always pass:
{
  "query": "what to search"
}
""";

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            query = new
            {
                type = "string",
                description = "Memory search query"
            },
            limit = new
            {
                type = "integer",
                description = "Optional result limit (default 5)"
            }
        },
        required = new[] { "query" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.ReadOnly;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        var query = GetString(context.Arguments, "query");

        if (string.IsNullOrWhiteSpace(query))
            return ToolResult.Failure("Query is required.");

        var limit = GetInt(context.Arguments, "limit") ?? 5;

        var facts = await memory.SearchAsync(
            context.UserId,
            query,
            limit,
            ct);

        if (facts.Count == 0)
        {
            return ToolResult.Success(
                "Ничего не нашла в памяти.",
                new
                {
                    query,
                    items = Array.Empty<object>()
                });
        }

        var items = facts.Select(x => new
        {
            x.Content,
            x.Kind,
            x.Importance,
            x.CreatedAt
        }).ToArray();

        return ToolResult.Success(
            $"Нашла {items.Length} записей в памяти.",
            new
            {
                query,
                items
            });
    }

    private static string? GetString(JsonElement json, string name)
    {
        return json.TryGetProperty(name, out var value) &&
               value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static int? GetInt(JsonElement json, string name)
    {
        return json.TryGetProperty(name, out var value) &&
               value.TryGetInt32(out var result)
            ? result
            : null;
    }
}