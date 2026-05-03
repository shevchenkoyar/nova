using Nova.Common.Application.Tools;
using Nova.Modules.HomeAssistant.Application.Resolver;
using Nova.Modules.HomeAssistant.Contracts;

namespace Nova.Modules.HomeAssistant.Application.Tools;

public sealed class ControlHomeAssistantTool(
    IHomeAssistantCommandResolver resolver,
    IHomeAssistantClient client) : INovaTool
{
    public string Name => "home_assistant.control";

    public string Description =>
        "Controls Home Assistant devices using natural language command.";

    public string UsageRules => """
Use this tool for natural language smart home control.

MUST use this tool when the user asks to control smart home devices:
- "включи свет"
- "выключи свет"
- "включи телевизор"
- "выключи телевизор"
- "запусти сцену"
Never answer that you cannot control smart home if this tool is available.

Always pass:
{ "command": "original user smart home command" }

Do not invent entity ids yourself.
This tool resolves the correct Home Assistant entity and service internally.
""";

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            command = new
            {
                type = "string",
                description = "Original natural language smart home command."
            }
        },
        required = new[] { "command" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.SafeAction;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("command", out var commandElement))
            return ToolResult.Failure("Command is required.");

        var command = commandElement.GetString();

        if (string.IsNullOrWhiteSpace(command))
            return ToolResult.Failure("Command is empty.");

        var resolution = await resolver.ResolveAsync(command, ct);

        if (!resolution.IsResolved)
        {
            return ToolResult.Failure(
                resolution.Reason ?? "Не смогла определить устройство или действие.");
        }

        if (string.IsNullOrWhiteSpace(resolution.EntityId))
            return ToolResult.Failure("Resolved entityId is empty.");

        if (string.IsNullOrWhiteSpace(resolution.Domain))
            return ToolResult.Failure("Resolved domain is empty.");

        if (string.IsNullOrWhiteSpace(resolution.Service))
            return ToolResult.Failure("Resolved service is empty.");

        await client.CallServiceAsync(
            new CallHomeAssistantServiceRequest(
                UserId: context.UserId,
                Domain: resolution.Domain,
                Service: resolution.Service,
                EntityId: resolution.EntityId,
                Data: resolution.Data),
            ct);

        return ToolResult.Success(
            $"Выполнила: {resolution.Domain}.{resolution.Service} для {resolution.EntityId}",
            resolution);
    }
}