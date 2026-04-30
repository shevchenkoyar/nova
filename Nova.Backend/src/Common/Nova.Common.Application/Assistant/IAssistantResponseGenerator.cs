using Nova.Common.Application.Tools;

namespace Nova.Common.Application.Assistant;

public interface IAssistantResponseGenerator
{
    Task<string> GenerateAsync(
        string userMessage,
        AssistantContext context,
        ToolExecutionResult? toolExecutionResult,
        CancellationToken cancellationToken);
}