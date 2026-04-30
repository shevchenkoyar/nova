using Nova.Common.Application.Tools;
using Nova.Contracts.Assistant;
using Nova.Modules.Relationships.Contracts;

namespace Nova.Common.Application.Assistant;

public sealed class AssistantService(
    IAssistantPlanner planner,
    INovaToolRegistry toolRegistry,
    ToolExecutor toolExecutor,
    ContextBuilder contextBuilder,
    IAssistantResponseGenerator responseGenerator)
{
    public async Task<AssistantMessageResponse> HandleAsync(
        AssistantMessageRequest request,
        CancellationToken ct)
    {
        var context = await contextBuilder.BuildAsync(
            request.UserId,
            request.Text,
            ct);

        if (context.AccessLevel == RelationshipAccessLevel.Blocked &&
            !IsRecoveryRequest(request.Text))
        {
            return new AssistantMessageResponse(
                Message: RelationshipAccessPolicy.BuildBlockedMessage(context),
                Data: new
                {
                    context.AccessLevel,
                    context.Relationship
                });
        }

        var tools = toolRegistry.GetDescriptors();

        var plannerResult = await planner.BuildPlanAsync(
            request.Text,
            tools,
            context,
            ct);

        if (!plannerResult.IsSuccess)
        {
            var message = await responseGenerator.GenerateAsync(
                request.Text,
                context,
                toolExecutionResult: null,
                ct);

            return new AssistantMessageResponse(
                Message: message,
                Data: new
                {
                    plannerResult.Error,
                    context.AccessLevel,
                    context.Relationship
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

            var message = await responseGenerator.GenerateAsync(
                request.Text,
                context,
                executionResult,
                ct);

            return new AssistantMessageResponse(
                Message: message,
                Data: new
                {
                    executionResult.Data,
                    executionResult.Steps,
                    executionResult.Error,
                    context.AccessLevel,
                    context.Relationship
                });
        }

        var directMessage = await responseGenerator.GenerateAsync(
            request.Text,
            context,
            toolExecutionResult: null,
            ct);

        return new AssistantMessageResponse(
            Message: directMessage,
            Data: new
            {
                context.AccessLevel,
                context.Relationship
            });
    }

    private static bool IsRecoveryRequest(string text)
    {
        return text.Contains("извини", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("прости", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("sorry", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("apolog", StringComparison.OrdinalIgnoreCase);
    }
}