using Nova.Common.Application.Tools;
using Nova.Modules.Relationships.Contracts;

namespace Nova.Modules.Relationships.Application.Tools;

public sealed class GetRelationshipStatusTool(
    IRelationshipsModuleApi relationships)
    : INovaTool
{
    public string Name => "relationships.get_status";

    public string Description =>
        "Gets Nova's relationship and reputation status for the current person.";

    public string UsageRules => """
                                Use this tool when the user asks about relationship status, trust, reputation,
                                whether Nova is offended, or how Nova currently feels about this person.

                                Russian triggers:
                                - "какие у нас отношения"
                                - "ты на меня обиделась"
                                - "какая у меня репутация"
                                - "ты мне доверяешь"

                                Always pass arguments as:
                                { }
                                """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new { }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.ReadOnly;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        var profile = await relationships.GetOrCreateAsync(
            context.UserId,
            ct);

        var message = profile.AccessLevel switch
        {
            RelationshipAccessLevel.Full => "У нас хорошие отношения.",
            RelationshipAccessLevel.Limited => "У нас нормальные отношения, но я чуть осторожнее.",
            RelationshipAccessLevel.BasicOnly => "Отношения напряжённые. Я буду помогать ограниченно.",
            RelationshipAccessLevel.ReadOnly => "Я сильно ограничиваю помощь из-за накопленных нарушений.",
            RelationshipAccessLevel.Blocked => "Я сейчас блокирую помощь для этого профиля, кроме критичных случаев.",
            _ => "Статус отношений неизвестен."
        };

        return ToolResult.Success(message, profile);
    }
}