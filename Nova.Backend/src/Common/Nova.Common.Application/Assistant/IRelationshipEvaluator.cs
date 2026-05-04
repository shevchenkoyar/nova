using Nova.Common.Application.Relationships;

namespace Nova.Common.Application.Assistant;


public interface IRelationshipEvaluator
{
    Task<RelationshipAdjustment> EvaluateAsync(
        Guid userId,
        string userMessage,
        AssistantContext? context,
        CancellationToken ct);
}