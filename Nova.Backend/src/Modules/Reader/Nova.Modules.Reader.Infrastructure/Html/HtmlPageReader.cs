using System.Net;
using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Nova.Modules.Reader.Contracts;

namespace Nova.Modules.Reader.Infrastructure.Html;

public sealed class HtmlPageReader(
    HttpClient httpClient,
    IOptions<ReaderInfrastructureOptions> options)
    : IPageReader
{
    public async Task<ReadPageResult> FetchAsync(
        FetchUrlRequest request,
        CancellationToken ct)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri))
            throw new InvalidOperationException($"Invalid URL: {request.Url}");

        if (uri.Scheme is not ("http" or "https"))
            throw new InvalidOperationException("Only http and https URLs are supported.");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        httpRequest.Headers.UserAgent.ParseAdd(options.Value.UserAgent);
        httpRequest.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,text/plain;q=0.9,*/*;q=0.5");

        using var response = await httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.MediaType;

        await using var stream = await response.Content.ReadAsStreamAsync(ct);

        using var limitedStream = new LimitedReadStream(
            stream,
            options.Value.MaxResponseBytes);

        using var reader = new StreamReader(
            limitedStream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true);

        var raw = await reader.ReadToEndAsync(ct);

        if (IsPlainText(contentType))
        {
            return new ReadPageResult(
                request.Url,
                Title: null,
                Text: TrimText(WebUtility.HtmlDecode(raw), request.MaxTextLength),
                ContentType: contentType);
        }

        var document = new HtmlDocument();
        document.LoadHtml(raw);

        RemoveNodes(document, "//script|//style|//noscript|//svg|//canvas|//iframe|//header|//footer|//nav");

        var title = document.DocumentNode
            .SelectSingleNode("//title")
            ?.InnerText;

        var body = document.DocumentNode.SelectSingleNode("//body")
                   ?? document.DocumentNode;

        var text = WebUtility.HtmlDecode(body.InnerText);
        text = NormalizeWhitespace(text);

        return new ReadPageResult(
            request.Url,
            NormalizeWhitespace(title ?? string.Empty),
            TrimText(text, request.MaxTextLength),
            contentType);
    }

    private static bool IsPlainText(string? contentType)
    {
        return contentType?.Contains("text/plain", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static void RemoveNodes(HtmlDocument document, string xpath)
    {
        var nodes = document.DocumentNode.SelectNodes(xpath);

        if (nodes is null)
            return;

        foreach (var node in nodes)
        {
            node.Remove();
        }
    }

    private static string NormalizeWhitespace(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var builder = new StringBuilder(text.Length);
        var previousWasWhitespace = false;

        foreach (var c in text)
        {
            if (char.IsWhiteSpace(c))
            {
                if (!previousWasWhitespace)
                {
                    builder.Append(' ');
                    previousWasWhitespace = true;
                }
            }
            else
            {
                builder.Append(c);
                previousWasWhitespace = false;
            }
        }

        return builder.ToString().Trim();
    }

    private static string TrimText(string text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return text.Length <= maxLength
            ? text
            : text[..maxLength];
    }
}