using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Application.Decorators;
using Nova.Common.Application.Messaging;

namespace Nova.Common.Application;

public static class ApplicationConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication(Assembly[] moduleAssemblies)
        {
            services
                .RegisterCqrsHandlers(moduleAssemblies)
                .RegisterCqrsDecorators();

            services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

            return services;
        }

        private IServiceCollection RegisterCqrsHandlers(Assembly[] moduleAssemblies)
        {
            return services.Scan(scan => scan.FromAssemblies(moduleAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }

        private IServiceCollection RegisterCqrsDecorators()
        {
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandHandler<>));
        
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandHandler<>));
            
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(IdempotentDecorator.CommandHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(IdempotentDecorator.CommandHandler<>));
        
            services.TryDecorate(typeof(IQueryHandler<,>), typeof(ExceptionHandlingDecorator.QueryHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<,>), typeof(ExceptionHandlingDecorator.CommandHandler<,>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(ExceptionHandlingDecorator.CommandHandler<>));

            return services;
        }
    }
}