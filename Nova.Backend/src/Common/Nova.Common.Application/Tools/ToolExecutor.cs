using System.Text.Json;
using Nova.Common.Application.Assistant;

namespace Nova.Common.Application.Tools;

public sealed class ToolExecutor(INovaToolRegistry registry)
{
    public async Task<ToolExecutionResult> ExecutePlanAsync(
        AssistantPlan plan,
        Guid userId,
        AssistantContext assistantContext,
        CancellationToken ct)
    {
        var stepResults = new List<ToolExecutionStepResult>();

        foreach (var step in plan.Steps)
        {
            var tool = registry.Find(step.ToolName);

            if (tool is null)
            {
                return ToolExecutionResult.Failure(
                    $"Tool '{step.ToolName}' not found.",
                    stepResults);
            }

            var arguments = JsonSerializer.SerializeToElement(step.Arguments);

            if (tool.SafetyLevel == ToolSafetyLevel.Forbidden)
            {
                var error = $"Tool '{tool.Name}' is forbidden.";

                stepResults.Add(ToolExecutionStepResult.Failure(
                    tool.Name,
                    arguments,
                    error));

                return ToolExecutionResult.Failure(error, stepResults);
            }

            if (tool.SafetyLevel is ToolSafetyLevel.Dangerous or ToolSafetyLevel.ConfirmationRequired)
            {
                var error = $"Tool '{tool.Name}' requires confirmation.";

                stepResults.Add(ToolExecutionStepResult.Failure(
                    tool.Name,
                    arguments,
                    error));

                return ToolExecutionResult.Failure(error, stepResults);
            }

            if (!ToolArgumentValidator.Validate(
                    tool.ParametersSchema,
                    arguments,
                    out var validationError))
            {
                var error = $"Invalid arguments for '{tool.Name}': {validationError}";

                stepResults.Add(ToolExecutionStepResult.Failure(
                    tool.Name,
                    arguments,
                    error));

                return ToolExecutionResult.Failure(error, stepResults);
            }

            var context = new ToolExecutionContext(
                UserId: userId,
                Arguments: arguments,
                AssistantContext: assistantContext);

            var result = await tool.ExecuteAsync(context, ct);

            if (!result.IsSuccess)
            {
                var error = result.Message ?? $"Tool '{tool.Name}' failed.";

                stepResults.Add(ToolExecutionStepResult.Failure(
                    tool.Name,
                    arguments,
                    error));

                return ToolExecutionResult.Failure(error, stepResults);
            }

            stepResults.Add(ToolExecutionStepResult.Success(
                tool.Name,
                arguments,
                result));
        }

        var last = stepResults.LastOrDefault();

        return ToolExecutionResult.Success(
            message: last?.Message ?? "Готово.",
            data: last?.Data,
            steps: stepResults);
    }
}