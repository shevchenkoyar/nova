namespace Nova.Modules.Relationships.Contracts;

public sealed record RegisterInteractionRequest(
    Guid PersonId,
    RelationshipInteractionKind Kind,
    string? Reason = null);