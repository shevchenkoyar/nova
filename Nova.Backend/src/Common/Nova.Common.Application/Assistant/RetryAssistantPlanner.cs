using Nova.Common.Application.Tools;

namespace Nova.Common.Application.Assistant;

public sealed class RetryAssistantPlanner(IAssistantPlanner inner) : IAssistantPlanner
{
    public async Task<PlannerResult> BuildPlanAsync(
        string text,
        IReadOnlyCollection<ToolDescriptor> tools,
        AssistantContext context,
        CancellationToken ct)
    {
        PlannerResult last = PlannerResult.Failure("Planner was not executed.");

        for (var attempt = 1; attempt <= 2; attempt++)
        {
            last = await inner.BuildPlanAsync(text, tools, context, ct);

            if (last.IsSuccess)
                return last;
        }

        return last;
    }
}