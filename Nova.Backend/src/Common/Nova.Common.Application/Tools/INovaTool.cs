namespace Nova.Common.Application.Tools;

public interface INovaTool
{
    string Name { get; }

    string Description { get; }

    string UsageRules { get; }

    object ParametersSchema { get; }

    ToolSafetyLevel SafetyLevel { get; }

    Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct);
}