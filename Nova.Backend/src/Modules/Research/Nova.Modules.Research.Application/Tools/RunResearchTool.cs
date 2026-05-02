using System.Text.Json;
using Nova.Common.Application.Tools;
using Nova.Modules.Research.Contracts;

namespace Nova.Modules.Research.Application.Tools;

public sealed class RunResearchTool(IResearchService researchService) : INovaTool
{
    public string Name => "research.run";

    public string Description =>
        "Runs research on a topic by searching the web, reading sources, and creating a synthesized summary.";

    public string UsageRules => """
        Use this tool when the user asks to research, study, investigate, analyze a topic,
        or asks to find information and make a summary.

        Russian triggers:
        - "изучи тему"
        - "поищи и сделай сводку"
        - "исследуй"
        - "разберись"
        - "найди информацию и объясни"
        - "собери информацию"

        English triggers:
        - "research"
        - "investigate"
        - "study this topic"
        - "find information and summarize"

        Always pass arguments as:
        { "topic": "research topic", "depth": "Standard", "language": "ru", "maxSources": 5 }

        Valid depth values:
        - Quick
        - Standard
        - Deep

        Do not use search.web separately if the user asks for a researched summary.
        Use research.run instead.
        
        Use this tool when the user asks to search/find information AND summarize/explain/analyze it.
        Russian triggers:
        - "поищи ... и сделай сводку"
        - "найди информацию и объясни"
        - "изучи тему"
        - "разберись"
        - "собери информацию"
        """;

    public object ParametersSchema => new
    {
        type = "object",
        properties = new
        {
            topic = new
            {
                type = "string",
                description = "Research topic."
            },
            depth = new
            {
                type = "string",
                description = "Research depth: Quick, Standard, or Deep."
            },
            language = new
            {
                type = "string",
                description = "Output language. Example: ru, en."
            },
            maxSources = new
            {
                type = "integer",
                description = "Maximum number of sources to read."
            }
        },
        required = new[] { "topic" }
    };

    public ToolSafetyLevel SafetyLevel => ToolSafetyLevel.ReadOnly;

    public async Task<ToolResult> ExecuteAsync(
        ToolExecutionContext context,
        CancellationToken ct)
    {
        if (!context.Arguments.TryGetProperty("topic", out var topicElement))
            return ToolResult.Failure("Topic is required.");

        var topic = topicElement.GetString();

        if (string.IsNullOrWhiteSpace(topic))
            return ToolResult.Failure("Topic is empty.");

        var depth = ResearchDepth.Standard;

        if (context.Arguments.TryGetProperty("depth", out var depthElement) &&
            depthElement.ValueKind == JsonValueKind.String &&
            !string.IsNullOrWhiteSpace(depthElement.GetString()) &&
            Enum.TryParse<ResearchDepth>(depthElement.GetString(), ignoreCase: true, out var parsedDepth))
        {
            depth = parsedDepth;
        }

        var language = "ru";

        if (context.Arguments.TryGetProperty("language", out var languageElement) &&
            languageElement.ValueKind == JsonValueKind.String &&
            !string.IsNullOrWhiteSpace(languageElement.GetString()))
        {
            language = languageElement.GetString()!;
        }

        var maxSources = depth switch
        {
            ResearchDepth.Quick => 3,
            ResearchDepth.Standard => 5,
            ResearchDepth.Deep => 8,
            _ => 5
        };

        if (context.Arguments.TryGetProperty("maxSources", out var maxSourcesElement) &&
            maxSourcesElement.ValueKind == JsonValueKind.Number &&
            maxSourcesElement.TryGetInt32(out var parsedMaxSources))
        {
            maxSources = Math.Clamp(parsedMaxSources, 1, 10);
        }

        var result = await researchService.RunAsync(
            new RunResearchRequest(
                UserId: context.UserId,
                Topic: topic.Trim(),
                Depth: depth,
                Language: language,
                MaxSources: maxSources),
            ct);

        return ToolResult.Success(
            $"Готова сводка по теме: {result.Topic}",
            result);
    }
}