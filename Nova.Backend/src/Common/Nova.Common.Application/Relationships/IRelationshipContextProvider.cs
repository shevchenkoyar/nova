namespace Nova.Common.Application.Relationships;

public interface IRelationshipContextProvider
{
    Task<AssistantRelationshipContext> GetOrCreateAsync(
        Guid userId,
        CancellationToken ct);

    Task<AssistantRelationshipContext> AdjustAsync(
        Guid userId,
        RelationshipAdjustment adjustment,
        CancellationToken ct);
}