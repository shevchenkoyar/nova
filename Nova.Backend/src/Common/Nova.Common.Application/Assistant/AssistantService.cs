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

        var plannerResult = await planner.BuildPlanAsync(
            request.Text,
            tools,
            context,
            ct);

        if (!plannerResult.IsSuccess)
        {
            return new AssistantMessageResponse(
                Message: "Я не смогла построить план действия.",
                Data: new
                {
                    plannerResult.Error
                });
        }

        var plan = plannerResult.Plan;

        if (plan is not null && plan.Steps.Count > 0)
        {
            var executionResult = await toolExecutor.ExecutePlanAsync(
                plan,
                request.UserId,
                context,
                ct);

            if (!executionResult.IsSuccess)
            {
                return new AssistantMessageResponse(
                    Message: "Я не смогла выполнить действие.",
                    Data: new
                    {
                        executionResult.Error,
                        executionResult.Steps
                    });
            }

            return new AssistantMessageResponse(
                Message: executionResult.Message ?? "Готово.",
                Data: new
                {
                    executionResult.Data,
                    executionResult.Steps
                });
        }

        return new AssistantMessageResponse(
            Message: BuildDirectAnswer(request.Text, context),
            Data: null);
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

        if (text.Contains("привет", StringComparison.OrdinalIgnoreCase))
        {
            return context.ResponseStyle == ResponseStyle.Short
                ? "Привет."
                : "Привет! Я Nova. Пока умею работать с памятью, поиском и простыми ответами.";
        }

        return context.ResponseStyle == ResponseStyle.Short
            ? "Пока не знаю."
            : "Пока не знаю, как ответить на это без дополнительных модулей.";
    }
}