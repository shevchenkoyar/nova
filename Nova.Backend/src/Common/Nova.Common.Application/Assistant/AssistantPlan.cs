namespace Nova.Common.Application.Assistant;

public sealed class AssistantPlan
{
    public List<AssistantStep> Steps { get; init; } = [];

    public bool RequiresConfirmation { get; init; }
}