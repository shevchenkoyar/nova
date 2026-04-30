using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nova.Common.Application.Assistant;
using Nova.Common.Presentation.Endpoints;
using Nova.Contracts.Assistant;

namespace Nova.Modules.Conversation.Presentation.Conversation;

[UsedImplicitly]
internal sealed class CreateMessage : IEndpoint
{
    internal const string EndpointName = "CreateMessage";
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("messages", async (
                AssistantMessageRequest request,
                AssistantService assistant,
                CancellationToken cancellationToken) =>
            {
                var response = await assistant.HandleAsync(request, cancellationToken);

                return Results.Ok(response);
            })
            .ProducesValidationProblem()
            .WithName(EndpointName)
            .WithTags(Tags.Conversation);
    }
}