namespace Nova.Modules.Relationships.Contracts;

public interface IRelationshipsModuleApi
{
    Task<RelationshipProfileDto> GetOrCreateAsync(
        Guid personId,
        CancellationToken ct);

    Task<RelationshipProfileDto> RegisterInteractionAsync(
        RegisterInteractionRequest request,
        CancellationToken ct);
    
    Task<RelationshipProfileDto> AdjustAsync(
        AdjustRelationshipRequest request,
        CancellationToken ct);
}