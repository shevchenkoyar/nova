using System.Text.Json;
using Nova.Common.Application.Assistant;
using OpenAI.Chat;

namespace Nova.Common.Application.Tools;

public sealed class OpenAiPlanner(ChatClient client) : IAssistantPlanner
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<PlannerResult> BuildPlanAsync(
        string text,
        IReadOnlyCollection<ToolDescriptor> tools,
        AssistantContext context,
        CancellationToken ct)
    {
        var systemPrompt = BuildSystemPrompt(tools, context);

        var response = await client.CompleteChatAsync(
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(text)
            ],
            cancellationToken: ct);

        var content = response.Value.Content[0].Text;

        return ParsePlan(content);
    }

    private static string BuildSystemPrompt(
        IReadOnlyCollection<ToolDescriptor> tools,
        AssistantContext context)
    {
        var toolList = string.Join("\n\n",
            tools.Select(t =>
            {
                var schemaJson = JsonSerializer.Serialize(
                    t.ParametersSchema,
                    JsonOptions);

                return $$"""
Tool: {{t.Name}}
Description: {{t.Description}}

Usage rules:
{{t.UsageRules}}

Arguments JSON schema:
{{schemaJson}}
""";
            }));

        var memoryBlock = context.RelevantMemory.Count == 0
            ? "No relevant memory."
            : string.Join("\n", context.RelevantMemory.Select(x => $"- {x.Content}"));

        return $$"""
You are an AI assistant planner.
Your job is to decide whether the user's request requires tools.

User context:
Response style: {{context.ResponseStyle}}

Relevant memory:
{{memoryBlock}}

Available tools:
{{toolList}}

Return ONLY valid JSON.

If tools are needed, return:
{
  "steps": [
    {
      "tool": "tool.name",
      "arguments": { }
    }
  ]
}

If no tools are needed, return:
null

Rules:
- Use only tools from the available tools list.
- Follow each tool's usage rules.
- Arguments MUST match the tool's JSON schema.
- Do not explain.
- Do not use markdown.
- Do not wrap JSON in ```json.
- If arguments do not match schema, fix them before returning.
""";
    }

    private static PlannerResult ParsePlan(string content)
    {
        try
        {
            content = content.Trim();

            if (content.Equals("null", StringComparison.OrdinalIgnoreCase))
                return PlannerResult.Success(null);

            var start = content.IndexOf('{');
            var end = content.LastIndexOf('}');

            if (start < 0 || end < 0 || end <= start)
                return PlannerResult.Failure($"Planner returned invalid JSON: {content}");

            var jsonText = content[start..(end + 1)];

            using var json = JsonDocument.Parse(jsonText);

            if (!json.RootElement.TryGetProperty("steps", out var stepsElement))
                return PlannerResult.Failure("Planner JSON does not contain 'steps'.");

            var steps = stepsElement
                .EnumerateArray()
                .Select(x =>
                {
                    var tool = x.GetProperty("tool").GetString();

                    if (string.IsNullOrWhiteSpace(tool))
                        throw new InvalidOperationException("Tool name is required.");

                    var arguments = JsonSerializer.Deserialize<object>(
                        x.GetProperty("arguments").GetRawText())!;

                    return new AssistantStep(tool, arguments);
                })
                .ToList();

            return PlannerResult.Success(
                steps.Count == 0 ? null : new AssistantPlan { Steps = steps });
        }
        catch (Exception ex)
        {
            return PlannerResult.Failure(ex.Message);
        }
    }
}