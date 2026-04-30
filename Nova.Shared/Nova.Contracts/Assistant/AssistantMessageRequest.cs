namespace Nova.Contracts.Assistant;

public sealed record AssistantMessageRequest(Guid UserId, string Text);