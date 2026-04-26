using FluentValidation;
using FluentValidation.Results;
using Nova.Common.Application.Messaging;
using Nova.Common.Domain;

namespace Nova.Common.Application.Decorators;

internal static class ValidationDecorator
{
    internal sealed class QueryHandler<TQuery, TResponse>(
        IEnumerable<IValidator<TQuery>> validators,
        IQueryHandler<TQuery, TResponse> innerHandler) : IQueryHandler<TQuery, TResponse> 
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(query, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(query, cancellationToken);
            }

            return Result.Failure<TResponse>(CreateValidationError(validationFailures));
        }
    }
    
    internal sealed class CommandHandler<TCommand, TResponse>(
        IEnumerable<IValidator<TCommand>> validators,
        ICommandHandler<TCommand, TResponse> innerHandler) : ICommandHandler<TCommand, TResponse> 
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            return Result.Failure<TResponse>(CreateValidationError(validationFailures));
        }
    }
    
    internal sealed class CommandHandler<TCommand>(
        IEnumerable<IValidator<TCommand>> validators,
        ICommandHandler<TCommand> innerHandler) : ICommandHandler<TCommand> 
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if (validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }
            
            return Result.Failure(CreateValidationError(validationFailures));
        }
    }
    
    private static async Task<ValidationFailure[]> ValidateAsync<TRequest>(
        TRequest request, 
        IEnumerable<IValidator<TRequest>> validators)
    {
        IValidator<TRequest>[] validatorsArray = validators as IValidator<TRequest>[] ?? validators.ToArray();
        
        if (!validatorsArray.Any())
        {
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validatorsArray.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }
    
    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}
