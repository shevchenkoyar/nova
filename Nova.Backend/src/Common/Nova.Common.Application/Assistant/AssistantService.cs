using Nova.Common.Application.Conversation;
using Nova.Common.Application.Relationships;
using Nova.Common.Application.Tools;
using Nova.Contracts.Assistant;

namespace Nova.Common.Application.Assistant;

public sealed class AssistantService(
    IAssistantPlanner planner,
    INovaToolRegistry toolRegistry,
    ToolExecutor toolExecutor,
    ContextBuilder contextBuilder,
    IAssistantResponseGenerator responseGenerator,
    IConversationHistory history,
    IRelationshipEvaluator relationshipEvaluator,
    IRelationshipContextProvider relationships)
{
    public async Task<AssistantMessageResponse> HandleAsync(
        AssistantMessageRequest request,
        CancellationToken ct)
    {
        await history.AddUserMessageAsync(
            request.UserId,
            request.Text,
            ct);

        var initialContext = await contextBuilder.BuildAsync(
            request.UserId,
            request.Text,
            ct);

        var relationshipDelta = await relationshipEvaluator.EvaluateAsync(
            request.UserId,
            request.Text,
            initialContext,
            ct);

        var updatedRelationship = await relationships.AdjustAsync(
            request.UserId,
            relationshipDelta,
            ct);

        var context = await contextBuilder.BuildAsync(
            request.UserId,
            request.Text,
            ct);

        if (context.AccessLevel == AssistantAccessLevel.Blocked &&
            !IsRecoveryRequest(request.Text))
        {
            var blockedMessage = RelationshipAccessPolicy.BuildBlockedMessage(context);

            var blockedResponse = new AssistantMessageResponse(
                Message: blockedMessage,
                Data: new
                {
                    RelationshipDelta = relationshipDelta,
                    Relationship = updatedRelationship,
                    context.AccessLevel
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
                plannerResult.Plan,
                RelationshipDelta = relationshipDelta,
                context.AccessLevel,
                context.Relationship
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
                    RelationshipDelta = relationshipDelta,
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
                new
                {
                    ExecutionResult = executionResult,
                    RelationshipDelta = relationshipDelta,
                    context.AccessLevel,
                    context.Relationship
                },
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
                    RelationshipDelta = relationshipDelta,
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
                RelationshipDelta = relationshipDelta,
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