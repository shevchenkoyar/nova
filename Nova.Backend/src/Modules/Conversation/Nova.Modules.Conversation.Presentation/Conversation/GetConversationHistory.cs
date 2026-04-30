using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nova.Common.Presentation.Endpoints;
using Nova.Contracts.Conversation;
using Nova.Modules.Conversation.Application;

namespace Nova.Modules.Conversation.Presentation.Conversation;

[UsedImplicitly]
internal sealed class GetConversationHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("messages/history/{userId:guid}", async (
            Guid userId,
            ConversationHistoryService history,
            CancellationToken cancellationToken) =>
        {
            var messages = await history.GetRecentAsync(
                userId,
                limit: 100,
                cancellationToken);

            var response = messages
                .Select(x => new ConversationMessageDto(
                    x.Id,
                    x.UserId,
                    x.Role.ToString(),
                    x.Content,
                    x.MetadataJson,
                    x.CreatedAt))
                .ToArray();
            return Results.Ok(response);
        });
    }
}