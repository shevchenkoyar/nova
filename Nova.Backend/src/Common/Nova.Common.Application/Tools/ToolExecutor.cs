using System.Text.Json;
using Nova.Common.Application.Assistant;

namespace Nova.Common.Application.Tools;

public sealed class ToolExecutor(INovaToolRegistry registry)
{
    public async Task<ToolResult> ExecutePlanAsync(
        AssistantPlan plan,
        Guid userId,
        AssistantContext assistantContext,
        CancellationToken ct)
    {
        ToolResult? last = null;

        foreach (var step in plan.Steps)
        {
            var tool = registry.Find(step.ToolName);

            if (tool is null)
                return ToolResult.Failure($"Tool '{step.ToolName}' not found.");

            if (tool.SafetyLevel is ToolSafetyLevel.Forbidden)
                return ToolResult.Failure($"Tool '{tool.Name}' is forbidden.");

            if (tool.SafetyLevel is ToolSafetyLevel.Dangerous or ToolSafetyLevel.ConfirmationRequired)
                return ToolResult.Failure($"Tool '{tool.Name}' requires confirmation.");

            var arguments = JsonSerializer.SerializeToElement(step.Arguments);

            if (!ToolArgumentValidator.Validate(tool.ParametersSchema, arguments, out var error))
                return ToolResult.Failure($"Invalid arguments for '{tool.Name}': {error}");

            var context = new ToolExecutionContext(
                userId,
                arguments,
                assistantContext);

            last = await tool.ExecuteAsync(context, ct);

            if (!last.IsSuccess)
                return last;
        }

        return last ?? ToolResult.Success();
    }
}