using Nova.Common.Application.Tools;
using Nova.Contracts.Assistant;
using Nova.Modules.Relationships.Contracts;

namespace Nova.Common.Application.Assistant;

public sealed class AssistantService(
    IAssistantPlanner planner,
    INovaToolRegistry toolRegistry,
    ToolExecutor toolExecutor,
    ContextBuilder contextBuilder,
    IAssistantResponseGenerator responseGenerator,
    IConversationHistory history)
{
    public async Task<AssistantMessageResponse> HandleAsync(
        AssistantMessageRequest request,
        CancellationToken ct)
    {
        await history.AddUserMessageAsync(
            request.UserId,
            request.Text,
            ct);

        var context = await contextBuilder.BuildAsync(
            request.UserId,
            request.Text,
            ct);

        if (context.AccessLevel == RelationshipAccessLevel.Blocked &&
            !IsRecoveryRequest(request.Text))
        {
            var blockedMessage = RelationshipAccessPolicy.BuildBlockedMessage(context);

            var blockedResponse = new AssistantMessageResponse(
                Message: blockedMessage,
                Data: new
                {
                    context.AccessLevel,
                    context.Relationship
                });

            await history.AddAssistantMessageAsync(
                request.UserId,
                blockedResponse.Message,
                blockedResponse.Data,
                ct);

            return blockedResponse;
        }

        var tools = toolRegistry.GetDescriptors();

        var plannerResult = await planner.BuildPlanAsync(
            request.Text,
            tools,
            context,
            ct);

        await history.AddPlannerMessageAsync(
            request.UserId,
            plannerResult.IsSuccess ? "Planner completed." : "Planner failed.",
            new
            {
                plannerResult.IsSuccess,
                plannerResult.Error,
                plannerResult.Plan
            },
            ct);

        if (!plannerResult.IsSuccess)
        {
            var message = await responseGenerator.GenerateAsync(
                request.Text,
                context,
                toolExecutionResult: null,
                ct);

            var response = new AssistantMessageResponse(
                Message: message,
                Data: new
                {
                    plannerResult.Error,
                    context.AccessLevel,
                    context.Relationship
                });

            await history.AddAssistantMessageAsync(
                request.UserId,
                response.Message,
                response.Data,
                ct);

            return response;
        }

        var plan = plannerResult.Plan;

        if (plan is not null && plan.Steps.Count > 0)
        {
            var executionResult = await toolExecutor.ExecutePlanAsync(
                plan,
                request.UserId,
                context,
                ct);

            await history.AddToolMessageAsync(
                request.UserId,
                executionResult.IsSuccess
                    ? "Tool execution completed."
                    : "Tool execution failed.",
                executionResult,
                ct);

            var message = await responseGenerator.GenerateAsync(
                request.Text,
                context,
                executionResult,
                ct);

            var response = new AssistantMessageResponse(
                Message: message,
                Data: new
                {
                    executionResult.Data,
                    executionResult.Steps,
                    executionResult.Error,
                    context.AccessLevel,
                    context.Relationship
                });

            await history.AddAssistantMessageAsync(
                request.UserId,
                response.Message,
                response.Data,
                ct);

            return response;
        }

        var directMessage = await responseGenerator.GenerateAsync(
            request.Text,
            context,
            toolExecutionResult: null,
            ct);

        var directResponse = new AssistantMessageResponse(
            Message: directMessage,
            Data: new
            {
                context.AccessLevel,
                context.Relationship
            });

        await history.AddAssistantMessageAsync(
            request.UserId,
            directResponse.Message,
            directResponse.Data,
            ct);

        return directResponse;
    }

    private static bool IsRecoveryRequest(string text)
    {
        return text.Contains("извини", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("прости", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("sorry", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("apolog", StringComparison.OrdinalIgnoreCase);
    }
}