using Nova.Common.Application.Tools;
using Nova.Modules.Reader.Contracts;

namespace Nova.Modules.Reader.Application.Tools;

public sealed class FetchUrlTool(IPageReader pageReader) : INovaTool
{
    public string Name => "reader.fetch_url";

    public string Description =>
        "Fetches a web page by URL and extracts readable text content.";

    public string UsageRules => """
        Use this tool when you need to read, inspect, analyze, or summarize a specific web page URL.

        Russian triggers:
        - "прочитай ссылку"
        - "изучи страницу"
        - "открой ссылку"
        - "посмотри что на странице"
        - "прочитай статью"

        English triggers:
        - "read this URL"
        - "open this page"
        - "fetch this page"
        - "summarize this article"

        Always pass arguments as:
        { "url": "https://example.com", "maxTextLength": 20000 }

        Do not use this tool for general web search. Use search.web first if the user did not provide a URL.
        """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            url = new
            {
                type = "string",
                description = "Absolute URL to fetch. Must start with http:// or https://."
            },
            maxTextLength = new
            {
                type = "integer",
                description = "Maximum extracted text length. Default is 20000."
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

        var maxTextLength = 20_000;

        if (context.Arguments.TryGetProperty("maxTextLength", out var maxTextLengthElement) &&
            maxTextLengthElement.ValueKind == System.Text.Json.JsonValueKind.Number &&
            maxTextLengthElement.TryGetInt32(out var parsedMaxTextLength))
        {
            maxTextLength = Math.Clamp(parsedMaxTextLength, 1_000, 50_000);
        }

        var result = await pageReader.FetchAsync(
            new FetchUrlRequest(
                context.UserId,
                url.Trim(),
                maxTextLength),
            ct);

        if (string.IsNullOrWhiteSpace(result.Text))
        {
            return ToolResult.Success(
                "Страница открылась, но текст извлечь не удалось.",
                result);
        }

        return ToolResult.Success(
            $"Прочитала страницу: {result.Title ?? result.Url}",
            result);
    }
}