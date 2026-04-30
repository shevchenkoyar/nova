namespace Nova.Common.Application.Tools;

public sealed record ToolResult(
    bool IsSuccess,
    string? Message = null,
    object? Data = null)
{
    public static ToolResult Success(string? message = null, object? data = null)
        => new(true, message, data);

    public static ToolResult Failure(string message)
        => new(false, message);
}