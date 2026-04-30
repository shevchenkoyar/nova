using Nova.Common.Application.Tools;
using Nova.Modules.Relationships.Contracts;

namespace Nova.Common.Application.Assistant;

public static class RelationshipAccessPolicy
{
    public static bool CanAnswer(AssistantContext context)
    {
        return context.AccessLevel != RelationshipAccessLevel.Blocked;
    }

    public static bool CanExecuteTool(
        AssistantContext context,
        ToolSafetyLevel safetyLevel)
    {
        return context.AccessLevel switch
        {
            RelationshipAccessLevel.Full => true,
            RelationshipAccessLevel.Limited =>
                safetyLevel is ToolSafetyLevel.ReadOnly or ToolSafetyLevel.SafeAction,
            RelationshipAccessLevel.BasicOnly =>
                safetyLevel is ToolSafetyLevel.ReadOnly,
            RelationshipAccessLevel.ReadOnly =>
                safetyLevel is ToolSafetyLevel.ReadOnly,
            _ => false
        };
    }

    public static string BuildBlockedMessage(AssistantContext context)
    {
        return context.AccessLevel switch
        {
            RelationshipAccessLevel.Blocked =>
                "Я сейчас не буду помогать этому профилю. Причина — слишком низкий уровень уважительного взаимодействия.",
            RelationshipAccessLevel.ReadOnly =>
                "Я могу только отвечать на безопасные вопросы, но не буду выполнять действия.",
            RelationshipAccessLevel.BasicOnly =>
                "Я помогу только с базовыми безопасными запросами.",
            _ => 
                "Я не могу выполнить это действие."
        };
    }
}