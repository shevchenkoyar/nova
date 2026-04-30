namespace Nova.Common.Application.Tools;

public sealed record ToolDescriptor(
    string Name,
    string Description,
    string UsageRules,
    object ParametersSchema,
    ToolSafetyLevel SafetyLevel);