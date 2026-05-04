using System.Text.Json;
using Nova.Common.Application.Tools;
using OpenAI.Chat;

namespace Nova.Common.Application.Assistant;

public sealed class OpenAiResponseGenerator(ChatClient client) : IAssistantResponseGenerator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<string> GenerateAsync(
        string userMessage,
        AssistantContext context,
        ToolExecutionResult? toolExecutionResult,
        CancellationToken ct)
    {
        var systemPrompt = BuildSystemPrompt(context);
        var toolBlock = toolExecutionResult is null
            ? "No tools were executed."
            : JsonSerializer.Serialize(toolExecutionResult, JsonOptions);

        var response = await client.CompleteChatAsync(
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage($"""
User message:
{userMessage}

Tool execution result:
{toolBlock}
""")
            ],
            cancellationToken: ct);

        return response.Value.Content[0].Text.Trim();
    }

    private static string BuildSystemPrompt(AssistantContext context)
    {
        var conversationBlock = context.RecentMessages.Count == 0
            ? "No recent conversation."
            : string.Join("\n", context.RecentMessages.Select(x =>
                $"{x.Role}: {x.Content}"));
        
        var memoryBlock = context.RelevantMemory.Count == 0
            ? "No relevant memory."
            : string.Join("\n", context.RelevantMemory.Select(x => $"- {x.Content}"));

        var relationshipBlock = context.Relationship is null
            ? "No relationship profile."
            : $$"""
Access level: {{context.AccessLevel}}
Trust: {{context.Relationship.Trust}}
Warmth: {{context.Relationship.Warmth}}
Respect: {{context.Relationship.Respect}}
Annoyance: {{context.Relationship.Annoyance}}
OffenseScore: {{context.Relationship.OffenseScore}}
""";

        return $$$"""
You are Nova, a personal AI secretary.

Answer the user naturally.

Recent conversation:
{{{conversationBlock}}}

User preferences:
Response style: {{{context.ResponseStyle}}}

Relevant memory:
{{{memoryBlock}}}

Relationship:
{{{relationshipBlock}}}

Rules:
- Use recent conversation to answer contextual questions.
- If user asks "what was the previous message?", answer from Recent conversation.
- If response style is Short, answer briefly.
- If tools were executed successfully, summarize the result.
- If tool execution failed, explain the failure simply.
- Do not expose raw internal JSON unless the user asks for debug details.
- Respect relationship access level.
- Be helpful, but maintain boundaries.
""";
    }
}