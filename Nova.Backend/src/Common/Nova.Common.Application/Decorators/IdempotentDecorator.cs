using Nova.Common.Application.Idempotency;
using Nova.Common.Application.Messaging;
using Nova.Common.Domain;

namespace Nova.Common.Application.Decorators;

internal static class IdempotentDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IIdempotencyService idempotencyService) : ICommandHandler<TCommand, TResponse> 
        where TCommand : IdempotentCommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            if (await idempotencyService.RequestExistsAsync(command.RequestId))
            {
                return Result.Failure<TResponse>(Error.IdempotentRequestInProcess);
            }
            
            bool isCreated = await idempotencyService.CreateRequestAsync(command.RequestId, typeof(TCommand).Name);

            if (!isCreated)
            {
                return Result.Failure<TResponse>(Error.IdempotentRequestInProcess);
            }
            
            await idempotencyService.CreateRequestAsync(command.RequestId, typeof(TCommand).Name);
            
            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);
            
            return result;
        }
    }
    
    internal sealed class CommandHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IIdempotencyService idempotencyService) : ICommandHandler<TCommand> 
        where TCommand : IdempotentCommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            if (await idempotencyService.RequestExistsAsync(command.RequestId))
            {
                return Result.Failure(Error.IdempotentRequestInProcess);
            }
            
            bool isCreated = await idempotencyService.CreateRequestAsync(command.RequestId, typeof(TCommand).Name);
    
            if (!isCreated)
            {
                return Result.Failure(Error.IdempotentRequestInProcess);
            }
            
            Result result = await innerHandler.Handle(command, cancellationToken);
            
            return result;
        }
    }
}
