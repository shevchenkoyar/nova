using Nova.Modules.Relationships.Contracts;

namespace Nova.Common.Application.Assistant;

public interface IRelationshipEvaluator
{
    Task<AdjustRelationshipRequest> EvaluateAsync(
        Guid userId,
        string userMessage,
        AssistantContext? context,
        CancellationToken ct);
}