using Nova.Common.Application.Tools;
using Nova.Modules.HomeAssistant.Application.Resolver;
using Nova.Modules.HomeAssistant.Contracts;

namespace Nova.Modules.HomeAssistant.Application.Tools;

public sealed class QueryHomeAssistantStateTool(
    IHomeAssistantCommandResolver resolver,
    IHomeAssistantClient client)
    : INovaTool
{
    public string Name => "home_assistant.query_state";

    public string Description =>
        "Checks Home Assistant entity state using natural language question.";

    public string UsageRules => """
Use this tool when the user asks about smart home device state.

Examples:
- "Выключен ли свет в прихожей?"
- "Свет в туалете включен?"
- "Телевизор сейчас работает?"
- "Какая температура в спальне?"

Always pass:
{ "question": "original user smart home state question" }

Do not answer directly if this tool is available.
""";

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            question = new
            {
                type = "string",
                description = "Original smart home state question."
            }
        },
        required = new[] { "question" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.ReadOnly;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("question", out var questionElement))
            return ToolResult.Failure("Question is required.");

        var question = questionElement.GetString();

        if (string.IsNullOrWhiteSpace(question))
            return ToolResult.Failure("Question is empty.");

        var resolution = await resolver.ResolveAsync(question, ct);

        if (!resolution.IsResolved || string.IsNullOrWhiteSpace(resolution.EntityId))
            return ToolResult.Failure(resolution.Reason ?? "Не смогла определить устройство.");

        var state = await client.GetStateAsync(resolution.EntityId, ct);

        if (state is null)
            return ToolResult.Failure($"Entity '{resolution.EntityId}' not found.");

        return ToolResult.Success(
            $"Состояние {resolution.EntityId}: {state.State}",
            new
            {
                resolution.EntityId,
                resolution.Reason,
                State = state
            });
    }
}