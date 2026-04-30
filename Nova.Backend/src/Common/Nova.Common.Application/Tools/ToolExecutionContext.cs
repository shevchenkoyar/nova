using System.Text.Json;
using Nova.Common.Application.Assistant;

namespace Nova.Common.Application.Tools;

public sealed record ToolExecutionContext(
    Guid UserId,
    JsonElement Arguments,
    AssistantContext AssistantContext);