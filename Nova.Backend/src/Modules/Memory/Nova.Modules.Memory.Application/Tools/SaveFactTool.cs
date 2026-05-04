using System.Text.Json;
using Nova.Common.Application.Tools;

namespace Nova.Modules.Memory.Application.Tools;

public sealed class SaveFactTool(MemoryService memory) : INovaTool
{
    public string Name => "memory.save_fact";

    public string Description => "Saves important user fact or preference into long-term memory.";

    public string UsageRules => """
Use this tool when user explicitly asks to remember/save something
or shares a stable preference useful in future.

Always pass:
{
  "text": "fact to remember",
  "kind": "preference",
  "importance": 80,
  "tags": ["response_style"]
}
""";

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            text = new { type = "string" },
            kind = new { type = "string" },
            importance = new { type = "integer" },
            tags = new { type = "array", items = new { type = "string" } }
        },
        required = new[] { "text" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.SafeAction;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        var text = GetString(context.Arguments, "text");

        if (string.IsNullOrWhiteSpace(text))
            return ToolResult.Failure("Text is required.");

        var kind = GetString(context.Arguments, "kind") ?? "fact";

        var importance = 50;

        if (context.Arguments.TryGetProperty("importance", out var importanceElement) &&
            importanceElement.TryGetInt32(out var parsedImportance))
        {
            importance = Math.Clamp(parsedImportance, 1, 100);
        }

        var tags = ReadTags(context.Arguments);

        await memory.SaveFactAsync(
            context.UserId,
            text,
            kind,
            importance,
            source: "assistant",
            tags,
            ct);

        return ToolResult.Success("Запомнила.", new
        {
            Text = text,
            Kind = kind,
            Importance = importance,
            Tags = tags
        });
    }

    private static string? GetString(JsonElement json, string name)
    {
        return json.TryGetProperty(name, out var value) &&
               value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static IReadOnlyList<string> ReadTags(JsonElement json)
    {
        if (!json.TryGetProperty("tags", out var value) ||
            value.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return value
            .EnumerateArray()
            .Where(x => x.ValueKind == JsonValueKind.String)
            .Select(x => x.GetString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .ToArray();
    }
}