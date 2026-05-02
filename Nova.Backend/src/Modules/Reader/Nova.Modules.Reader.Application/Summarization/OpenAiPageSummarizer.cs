using Nova.Modules.Reader.Contracts;
using OpenAI.Chat;

namespace Nova.Modules.Reader.Application.Summarization;

public sealed class OpenAiPageSummarizer(ChatClient client) : IPageSummarizer
{
    public async Task<PageSummaryResult> SummarizeAsync(
        ReadPageResult page,
        PageSummaryOptions options,
        CancellationToken ct)
    {
        var response = await client.CompleteChatAsync(
            [
                new SystemChatMessage(BuildSystemPrompt(options)),
                new UserChatMessage($"""
                                     URL:
                                     {page.Url}

                                     Title:
                                     {page.Title}

                                     Text:
                                     {page.Text}
                                     """)
            ],
            cancellationToken: ct);

        var summary = response.Value.Content[0].Text.Trim();

        return new PageSummaryResult(
            Url: page.Url,
            Title: page.Title,
            Summary: summary,
            KeyFacts: []);
    }

    private static string BuildSystemPrompt(PageSummaryOptions options)
    {
        return $$"""
                 You summarize web pages for Nova, a personal AI secretary.

                 Rules:
                 - Language: {{options.Language}}
                 - Be concise.
                 - Use at most {{options.MaxBullets}} bullet points.
                 - Extract only information supported by the page text.
                 - Do not invent facts.
                 - If the page text is weak or empty, say that clearly.
                 - Do not include raw HTML.
                 """;
    }
}