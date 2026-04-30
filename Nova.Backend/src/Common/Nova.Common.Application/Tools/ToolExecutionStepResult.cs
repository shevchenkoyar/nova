using System.Text.Json;

namespace Nova.Common.Application.Tools;

public sealed record ToolExecutionStepResult(
    string ToolName,
    JsonElement Arguments,
    bool IsSuccess,
    string? Message,
    object? Data,
    string? Error)
{
    public static ToolExecutionStepResult Success(
        string toolName,
        JsonElement arguments,
        ToolResult result)
    {
        return new ToolExecutionStepResult(
            ToolName: toolName,
            Arguments: arguments,
            IsSuccess: true,
            Message: result.Message,
            Data: result.Data,
            Error: null);
    }

    public static ToolExecutionStepResult Failure(
        string toolName,
        JsonElement arguments,
        string error)
    {
        return new ToolExecutionStepResult(
            ToolName: toolName,
            Arguments: arguments,
            IsSuccess: false,
            Message: null,
            Data: null,
            Error: error);
    }
}