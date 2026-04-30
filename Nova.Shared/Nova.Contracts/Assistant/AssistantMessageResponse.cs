namespace Nova.Contracts.Assistant;

public sealed record AssistantMessageResponse(
    string Message,
    object? Data = null);