using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nova.Common.Application.Messaging;
using Nova.Common.Domain;
using Serilog.Context;

namespace Nova.Common.Application.Decorators;

internal static class LoggingDecorator
{
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger) : IQueryHandler<TQuery, TResponse> 
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string moduleName = GetModuleName(typeof(TQuery).FullName!);
            string requestName = typeof(TQuery).Name;

            Activity.Current?.SetTag("request.module", moduleName);
            Activity.Current?.SetTag("request.name", requestName);

            using (LogContext.PushProperty("Module", moduleName))
            {
                logger.LogInformation("Processing request {RequestName}", requestName);

                Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Completed request {RequestName}", requestName);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        logger.LogError("Completed request {RequestName} with error", requestName);
                    }
                }

                return result;
            }
        }
    }
    
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger) : ICommandHandler<TCommand, TResponse> 
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string moduleName = GetModuleName(typeof(TCommand).FullName!);
            string requestName = typeof(TCommand).Name;

            Activity.Current?.SetTag("request.module", moduleName);
            Activity.Current?.SetTag("request.name", requestName);

            using (LogContext.PushProperty("Module", moduleName))
            {
                logger.LogInformation("Processing request {RequestName}", requestName);

                Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Completed request {RequestName}", requestName);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        logger.LogError("Completed request {RequestName} with error", requestName);
                    }
                }

                return result;
            }
        }
    }
    
    internal sealed class CommandHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandHandler<TCommand>> logger) : ICommandHandler<TCommand> 
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string moduleName = GetModuleName(typeof(TCommand).FullName!);
            string requestName = typeof(TCommand).Name;

            Activity.Current?.SetTag("request.module", moduleName);
            Activity.Current?.SetTag("request.name", requestName);

            using (LogContext.PushProperty("Module", moduleName))
            {
                logger.LogInformation("Processing request {RequestName}", requestName);

                Result result = await innerHandler.Handle(command, cancellationToken);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Completed request {RequestName}", requestName);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        logger.LogError("Completed request {RequestName} with error", requestName);
                    }
                }

                return result;
            }
        }
    }
    
    private static string GetModuleName(string requestName) => requestName.Split('.')[2];
}
