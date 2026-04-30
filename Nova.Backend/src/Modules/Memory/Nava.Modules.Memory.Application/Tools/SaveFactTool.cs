namespace Nava.Modules.Memory.Application.Tools;

using Contracts;
using Nova.Common.Application.Tools;

public sealed class SaveFactTool(IMemoryModuleApi memory) : INovaTool
{
    public string Name => "memory.save_fact";

    public string Description =>
        "Saves an important user fact or preference into long-term memory.";

    public string UsageRules => """
                                Use this tool when the user asks to remember, save, store, note, or memorize something.
                                Russian triggers: "запомни", "сохрани", "запиши".
                                Do not use this tool for temporary information.
                                Always pass arguments as:
                                { "text": "the fact or preference to remember" }
                                """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            text = new
            {
                type = "string",
                description = "The exact fact or preference to remember."
            }
        },
        required = new[] { "text" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.SafeAction;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("text", out var textElement))
            return ToolResult.Failure("Text is required.");

        var text = textElement.GetString();

        if (string.IsNullOrWhiteSpace(text))
            return ToolResult.Failure("Text is empty.");

        await memory.SaveFactAsync(
            new SaveMemoryFactRequest(
                context.UserId,
                text,
                Source: "user",
                Importance: MemoryImportance.High),
            ct);

        return ToolResult.Success("Запомнила.");
    }
}