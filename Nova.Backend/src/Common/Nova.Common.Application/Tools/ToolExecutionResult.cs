namespace Nova.Common.Application.Tools;

public sealed record ToolExecutionResult(
    bool IsSuccess,
    string? Message,
    object? Data,
    IReadOnlyList<ToolExecutionStepResult> Steps,
    string? Error)
{
    public static ToolExecutionResult Success(
        string? message,
        object? data,
        IReadOnlyList<ToolExecutionStepResult> steps)
    {
        return new ToolExecutionResult(
            IsSuccess: true,
            Message: message,
            Data: data,
            Steps: steps,
            Error: null);
    }

    public static ToolExecutionResult Failure(
        string error,
        IReadOnlyList<ToolExecutionStepResult> steps)
    {
        return new ToolExecutionResult(
            IsSuccess: false,
            Message: null,
            Data: null,
            Steps: steps,
            Error: error);
    }
}