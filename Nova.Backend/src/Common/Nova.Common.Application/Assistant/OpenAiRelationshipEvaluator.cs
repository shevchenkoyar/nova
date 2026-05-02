using System.Text.Json;
using Nova.Modules.Relationships.Contracts;
using OpenAI.Chat;

namespace Nova.Common.Application.Assistant;

public sealed class OpenAiRelationshipEvaluator(
    ChatClient client)
    : IRelationshipEvaluator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AdjustRelationshipRequest> EvaluateAsync(
        Guid userId,
        string userMessage,
        AssistantContext? context,
        CancellationToken ct)
    {
        var systemPrompt = BuildSystemPrompt(context);

        var response = await client.CompleteChatAsync(
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userMessage)
            ],
            cancellationToken: ct);

        var content = response.Value.Content[0].Text;

        var delta = ParseDelta(content);

        return new AdjustRelationshipRequest(
            PersonId: userId,
            TrustDelta: Clamp(delta.TrustDelta),
            WarmthDelta: Clamp(delta.WarmthDelta),
            RespectDelta: Clamp(delta.RespectDelta),
            FamiliarityDelta: Clamp(delta.FamiliarityDelta),
            AnnoyanceDelta: Clamp(delta.AnnoyanceDelta),
            OffenseDelta: Clamp(delta.OffenseDelta),
            Reason: string.IsNullOrWhiteSpace(delta.Reason)
                ? "LLM relationship evaluation."
                : delta.Reason);
    }

    private static string BuildSystemPrompt(
        AssistantContext? context)
    {
        var relationshipBlock = context?.Relationship is null
            ? "No current relationship profile."
            : $$"""
Current relationship:
Trust: {{context.Relationship.Trust}}
Warmth: {{context.Relationship.Warmth}}
Respect: {{context.Relationship.Respect}}
Familiarity: {{context.Relationship.Familiarity}}
Annoyance: {{context.Relationship.Annoyance}}
OffenseScore: {{context.Relationship.OffenseScore}}
AccessLevel: {{context.Relationship.AccessLevel}}
""";

        return $$"""
You evaluate how a user's message should affect Nova's relationship parameters.

{{relationshipBlock}}

Return ONLY valid JSON:
{
  "trustDelta": 0,
  "warmthDelta": 0,
  "respectDelta": 0,
  "familiarityDelta": 1,
  "annoyanceDelta": 0,
  "offenseDelta": 0,
  "reason": "short reason"
}

Parameters:
- trustDelta: affects trust in the user.
- warmthDelta: affects emotional warmth.
- respectDelta: affects perceived respect.
- familiarityDelta: increases with normal interaction.
- annoyanceDelta: increases when the user is demanding/rude; decreases after polite/apology.
- offenseDelta: increases on insults, aggression, boundary violations; decreases after sincere apology.

Rules:
- Normal neutral technical messages: familiarityDelta = 1, other deltas = 0.
- Polite messages: warmthDelta +1, respectDelta +1, familiarityDelta +1.
- Thanks/gratitude: warmthDelta +1, respectDelta +1, familiarityDelta +1.
- Apology: warmthDelta +2, respectDelta +2, annoyanceDelta -3, offenseDelta -3, familiarityDelta +1.
- Mild rudeness/demanding tone: respectDelta -1, annoyanceDelta +2, familiarityDelta +1.
- Insults/aggression: respectDelta -4, warmthDelta -3, annoyanceDelta +5, offenseDelta +5.
- Dangerous/manipulative/boundary violating request: trustDelta -4, respectDelta -3, offenseDelta +4.
- Do not punish directness in technical questions.
- Do not overreact.
- Keep normal deltas between -3 and +3.
- Use larger deltas only for clearly harmful, insulting, aggressive, or apologetic messages.
- Return JSON only. No markdown. No explanation.
""";
    }

    private static RelationshipDeltaDto ParseDelta(string content)
    {
        try
        {
            content = content.Trim();

            var start = content.IndexOf('{');
            var end = content.LastIndexOf('}');

            if (start < 0 || end < 0 || end <= start)
            {
                return RelationshipDeltaDto.Neutral(
                    "Failed to parse relationship evaluation JSON.");
            }

            var json = content[start..(end + 1)];

            var dto = JsonSerializer.Deserialize<RelationshipDeltaDto>(
                json,
                JsonOptions);

            return dto ?? RelationshipDeltaDto.Neutral(
                "Empty relationship evaluation result.");
        }
        catch
        {
            return RelationshipDeltaDto.Neutral(
                "Relationship evaluation failed.");
        }
    }

    private static int Clamp(int value)
    {
        return Math.Clamp(value, -10, 10);
    }

    private sealed record RelationshipDeltaDto(
        int TrustDelta,
        int WarmthDelta,
        int RespectDelta,
        int FamiliarityDelta,
        int AnnoyanceDelta,
        int OffenseDelta,
        string? Reason)
    {
        public static RelationshipDeltaDto Neutral(string reason)
        {
            return new RelationshipDeltaDto(
                TrustDelta: 0,
                WarmthDelta: 0,
                RespectDelta: 0,
                FamiliarityDelta: 1,
                AnnoyanceDelta: 0,
                OffenseDelta: 0,
                Reason: reason);
        }
    }
}