using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nova.Common.Application.Messaging;
using Nova.Common.Presentation.Endpoints;
using Nova.Common.Presentation.Idempotency;
using Nova.Common.Presentation.Results;
using Nova.Modules.Conversation.Application.Messages.Commands.CreateMessage;

namespace Nova.Modules.Conversation.Presentation.Conversation;

[UsedImplicitly]
internal sealed class CreateMessage : IEndpoint
{
    internal const string EndpointName = "CreateMessage";
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("messages", async (
            IdempotencyKey idempotencyKey,
            CreateMessageRequest request,
            ICommandHandler<CreateMessageCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new CreateMessageCommand(
                idempotencyKey, 
                request.Message), 
                cancellationToken);
            
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .ProducesValidationProblem()
        .WithName(EndpointName)
        .WithTags(Tags.Conversation)
        .RequireIdempotencyKey();
    }
    
    [UsedImplicitly]
    internal sealed class CreateMessageRequest
    {
        [Description("Echos back the message")]
        public required string Message { get; [UsedImplicitly] init; }
    }
}