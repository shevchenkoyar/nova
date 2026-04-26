using Nova.Common.Application.Messaging;

namespace Nova.Common.Application.Idempotency;

public abstract record IdempotentCommand(Guid RequestId) : ICommand;

public abstract record IdempotentCommand<TResponse>(Guid RequestId) : ICommand<TResponse>;