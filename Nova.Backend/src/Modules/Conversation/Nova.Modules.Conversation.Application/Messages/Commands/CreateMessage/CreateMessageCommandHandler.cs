using JetBrains.Annotations;
using Nova.Common.Application.Messaging;
using Nova.Common.Domain;

namespace Nova.Modules.Conversation.Application.Messages.Commands.CreateMessage;

[UsedImplicitly]
internal sealed class CreateMessageCommandHandler : ICommandHandler<CreateMessageCommand, string>
{
    public async Task<Result<string>> Handle(CreateMessageCommand command, CancellationToken cancellationToken)
    {
        return Result.Success($"Echo: {command.Message}");
    }
}