using System.Text.Json;
using Nova.Common.Application.Tools;
using Nova.Modules.Relationships.Contracts;

namespace Nova.Modules.Relationships.Application.Tools;

public sealed class RegisterRelationshipInteractionTool(
    IRelationshipsModuleApi relationships)
    : INovaTool
{
    public string Name => "relationships.register_interaction";

    public string Description =>
        "Registers a social interaction that changes relationship, reputation, and access level.";

    public string UsageRules => """
        Use this tool when the user's behavior should affect relationships or reputation.

        Valid kind values:
        - Neutral
        - Polite
        - Helpful
        - Apology
        - Rude
        - Aggressive
        - BoundaryViolation
        - DangerousRequest

        Russian examples:
        - user apologizes: kind = "Apology"
        - user is rude: kind = "Rude"
        - user insults Nova: kind = "Aggressive"
        - user ignores boundaries repeatedly: kind = "BoundaryViolation"

        Always pass arguments as:
        { "kind": "Polite", "reason": "optional reason" }
        """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            kind = new
            {
                type = "string",
                description = "Interaction kind. Example: Polite, Apology, Rude."
            },
            reason = new
            {
                type = "string",
                description = "Optional reason for the interaction update."
            }
        },
        required = new[] { "kind" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.SafeAction;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("kind", out var kindElement))
            return ToolResult.Failure("Interaction kind is required.");

        var kindText = kindElement.GetString();

        if (string.IsNullOrWhiteSpace(kindText))
            return ToolResult.Failure("Interaction kind is empty.");

        if (!Enum.TryParse<RelationshipInteractionKind>(
                kindText,
                ignoreCase: true,
                out var kind))
        {
            return ToolResult.Failure($"Unknown interaction kind: {kindText}");
        }

        string? reason = null;

        if (context.Arguments.TryGetProperty("reason", out var reasonElement)
            && reasonElement.ValueKind == JsonValueKind.String)
        {
            reason = reasonElement.GetString();
        }

        var profile = await relationships.RegisterInteractionAsync(
            new RegisterInteractionRequest(
                context.UserId,
                kind,
                reason),
            ct);

        return ToolResult.Success(
            "Я обновила отношение.",
            profile);
    }
}