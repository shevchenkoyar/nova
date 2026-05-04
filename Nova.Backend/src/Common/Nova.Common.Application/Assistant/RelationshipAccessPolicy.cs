using Nova.Common.Application.Relationships;
using Nova.Common.Application.Tools;

namespace Nova.Common.Application.Assistant;


public static class RelationshipAccessPolicy
{
    public static bool CanExecuteTool(
        AssistantContext context,
        ToolSafetyLevel safetyLevel)
    {
        return context.AccessLevel switch
        {
            AssistantAccessLevel.Full => true,

            AssistantAccessLevel.Limited =>
                safetyLevel is ToolSafetyLevel.ReadOnly or ToolSafetyLevel.SafeAction,

            AssistantAccessLevel.BasicOnly =>
                safetyLevel is ToolSafetyLevel.ReadOnly,

            AssistantAccessLevel.ReadOnly =>
                safetyLevel is ToolSafetyLevel.ReadOnly,

            AssistantAccessLevel.Blocked => false,

            _ => false
        };
    }

    public static string BuildBlockedMessage(
        AssistantContext context)
    {
        return context.AccessLevel switch
        {
            AssistantAccessLevel.Blocked =>
                "Я сейчас не буду помогать этому профилю. Причина — слишком низкий уровень уважительного взаимодействия.",

            AssistantAccessLevel.ReadOnly =>
                "Я могу только отвечать на безопасные вопросы, но не буду выполнять действия.",

            AssistantAccessLevel.BasicOnly =>
                "Я помогу только с базовыми безопасными запросами.",

            _ =>
                "Я не могу выполнить это действие."
        };
    }
}