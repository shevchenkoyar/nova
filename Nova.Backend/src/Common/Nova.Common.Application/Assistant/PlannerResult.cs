namespace Nova.Common.Application.Assistant;

public sealed record PlannerResult(
    bool IsSuccess,
    AssistantPlan? Plan,
    string? Error)
{
    public static PlannerResult Success(AssistantPlan? plan) =>
        new(true, plan, null);

    public static PlannerResult Failure(string error) =>
        new(false, null, error);
}