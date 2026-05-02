using Nova.Modules.Research.Contracts;
using OpenAI.Chat;

namespace Nova.Modules.Research.Application;

public sealed class OpenAiResearchSynthesizer(ChatClient client) : IResearchSynthesizer
{
    public async Task<string> SynthesizeAsync(
        string topic,
        IReadOnlyList<ResearchSourceSummary> sources,
        string language,
        CancellationToken ct)
    {
        if (sources.Count == 0)
        {
            return language.Equals("ru", StringComparison.OrdinalIgnoreCase)
                ? "Я нашла ссылки, но не смогла прочитать достаточно содержимого для надёжной сводки."
                : "I found links but could not read enough content to create a reliable summary.";
        }

        var sourceBlock = string.Join("\n\n", sources.Select((x, index) => $"""
                                                                            Source {index + 1}
                                                                            Title: {x.Title}
                                                                            URL: {x.Url}
                                                                            Summary:
                                                                            {x.Summary}
                                                                            """));

        var response = await client.CompleteChatAsync(
            [
                new SystemChatMessage($"""
                                       You are Nova's research synthesizer.

                                       Create a concise but useful research summary.

                                       Rules:
                                       - Language: {language}
                                       - Use only the provided source summaries.
                                       - Do not invent facts.
                                       - Mention uncertainty if sources are weak.
                                       - Structure:
                                         1. Краткий вывод / Brief conclusion
                                         2. Основные факты / Key facts
                                         3. Источники / Sources
                                       """),
                new UserChatMessage($"""
                                     Research topic:
                                     {topic}

                                     Sources:
                                     {sourceBlock}
                                     """)
            ],
            cancellationToken: ct);

        return response.Value.Content[0].Text.Trim();
    }
}