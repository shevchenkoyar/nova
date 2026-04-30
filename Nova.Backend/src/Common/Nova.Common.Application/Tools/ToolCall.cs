namespace Nova.Common.Application.Tools;

public sealed record ToolCall(
    string ToolName,
    object Arguments);