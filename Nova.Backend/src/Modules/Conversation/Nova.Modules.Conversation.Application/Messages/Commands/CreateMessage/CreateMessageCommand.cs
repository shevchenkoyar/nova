using Nova.Common.Application.Idempotency;

namespace Nova.Modules.Conversation.Application.Messages.Commands.CreateMessage;

public sealed record CreateMessageCommand(Guid RequestId, string Message) : IdempotentCommand<string>(RequestId);