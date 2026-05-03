using Nova.Common.Application.Clock;
using Nova.Common.Application.Tools;

namespace Nova.Modules.Clock.Application.Tools;

public sealed class SystemTimeTool(IDateTimeProvider clock) : INovaTool
{
    public string Name => "system.time_now";

    public string Description => "Gets current local date and time.";

    public string UsageRules => """
                                Use this tool when the user asks about current time, date, today, tomorrow, yesterday,
                                weekday, current month, or relative dates.

                                Russian triggers:
                                - "который час"
                                - "какое сегодня число"
                                - "какая сегодня дата"
                                - "сегодня"
                                - "завтра"
                                - "вчера"

                                Always pass:
                                { }
                                """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new { }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.ReadOnly;

    public Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        var now = clock.Now;

        return Task.FromResult(ToolResult.Success(
            $"Сейчас {now:dd.MM.yyyy HH:mm:ss zzz}",
            new
            {
                LocalDateTime = now,
                now.Date,
                Time = now.TimeOfDay,
                DayOfWeek = now.DayOfWeek.ToString(),
                UnixTimeSeconds = now.ToUnixTimeSeconds()
            }));
    }
}