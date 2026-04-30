namespace Nova.Common.Application.Assistant;

public sealed record AssistantStep(
    string ToolName,
    object Arguments);