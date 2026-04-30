using Nova.Common.Application.Tools;
using Nova.Contracts.Assistant;

namespace Nova.Common.Application.Assistant;

public sealed class AssistantService(
    IAssistantPlanner planner,
    INovaToolRegistry toolRegistry,
    ToolExecutor toolExecutor,
    ContextBuilder contextBuilder)
{
    public async Task<AssistantMessageResponse> HandleAsync(
        AssistantMessageRequest request,
        CancellationToken ct)
    {
        var context = await contextBuilder.BuildAsync(
            request.UserId,
            request.Text,
            ct);

        var tools = toolRegistry.GetDescriptors();

        var plan = await planner.BuildPlanAsync(
            request.Text,
            tools,
            context,
            ct);

        if (plan is not null && plan.Steps.Count > 0)
        {
            var toolResult = await toolExecutor.ExecutePlanAsync(
                plan,
                request.UserId,
                context,
                ct);

            return new AssistantMessageResponse(
                toolResult.Message ?? "Готово.",
                toolResult.Data);
        }

        return new AssistantMessageResponse(
            BuildDirectAnswer(request.Text, context));
    }

    private static string BuildDirectAnswer(
        string text,
        AssistantContext context)
    {
        if (text.Contains("dlms", StringComparison.OrdinalIgnoreCase))
        {
            return context.ResponseStyle == ResponseStyle.Short
                ? "DLMS — протокол обмена данными со счётчиками."
                : "DLMS — это протокол обмена данными со счётчиками. Он используется для чтения показаний, профилей, событий и управления устройствами.";
        }

        return context.ResponseStyle == ResponseStyle.Short
            ? "Пока не знаю."
            : "Пока не знаю, как ответить на это без дополнительных модулей.";
    }
}