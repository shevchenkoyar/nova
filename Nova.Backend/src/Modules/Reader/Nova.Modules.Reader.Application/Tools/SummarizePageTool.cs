using System.Text.Json;
using Nova.Common.Application.Tools;
using Nova.Modules.Reader.Application.Summarization;
using Nova.Modules.Reader.Contracts;

namespace Nova.Modules.Reader.Application.Tools;

public sealed class SummarizePageTool(
    IPageReader pageReader,
    IPageSummarizer summarizer)
    : INovaTool
{
    public string Name => "reader.summarize_page";

    public string Description =>
        "Fetches a web page by URL and creates a concise summary.";

    public string UsageRules => """
        Use this tool when the user asks to summarize, study, analyze, or explain a specific URL.

        Russian triggers:
        - "прочитай и кратко перескажи"
        - "сделай сводку по ссылке"
        - "изучи статью"
        - "объясни что в этой ссылке"
        - "суммаризируй страницу"

        English triggers:
        - "summarize this page"
        - "read and summarize"
        - "analyze this article"

        Always pass arguments as:
        { "url": "https://example.com", "language": "ru", "maxBullets": 7 }

        Do not use this tool for general search. Use search.web first if no URL is provided.
        """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            url = new
            {
                type = "string",
                description = "Absolute URL to fetch and summarize."
            },
            language = new
            {
                type = "string",
                description = "Summary language. Example: ru, en."
            },
            maxBullets = new
            {
                type = "integer",
                description = "Maximum number of bullet points."
            }
        },
        required = new[] { "url" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.ReadOnly;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("url", out var urlElement))
            return ToolResult.Failure("Url is required.");

        var url = urlElement.GetString();

        if (string.IsNullOrWhiteSpace(url))
            return ToolResult.Failure("Url is empty.");

        var language = "ru";

        if (context.Arguments.TryGetProperty("language", out var languageElement) &&
            languageElement.ValueKind == JsonValueKind.String &&
            !string.IsNullOrWhiteSpace(languageElement.GetString()))
        {
            language = languageElement.GetString()!;
        }

        var maxBullets = 7;

        if (context.Arguments.TryGetProperty("maxBullets", out var maxBulletsElement) &&
            maxBulletsElement.ValueKind == JsonValueKind.Number &&
            maxBulletsElement.TryGetInt32(out var parsedMaxBullets))
        {
            maxBullets = Math.Clamp(parsedMaxBullets, 3, 12);
        }

        var page = await pageReader.FetchAsync(
            new FetchUrlRequest(
                context.UserId,
                url.Trim(),
                MaxTextLength: 30_000),
            ct);

        var summary = await summarizer.SummarizeAsync(
            page,
            new PageSummaryOptions(
                Language: language,
                MaxBullets: maxBullets),
            ct);

        return ToolResult.Success(
            $"Сделала сводку по странице: {summary.Title ?? summary.Url}",
            summary);
    }
}