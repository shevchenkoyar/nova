using Nova.Common.Application.Tools;

namespace Nova.Common.Application.Assistant;

public interface IAssistantPlanner
{
    Task<AssistantPlan?> BuildPlanAsync(
        string text,
        IReadOnlyCollection<ToolDescriptor> tools,
        AssistantContext context,
        CancellationToken ct);
}