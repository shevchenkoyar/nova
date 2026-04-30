using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Application.Assistant;
using Nova.Common.Application.Decorators;
using Nova.Common.Application.Messaging;
using Nova.Common.Application.Tools;
using OpenAI.Chat;

namespace Nova.Common.Application;

public static class ApplicationConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication(Assembly[] moduleAssemblies, IConfigurationManager configuration)
        {
            // Temporary: register common services here. In the future, we can split it into multiple modules if needed.
            services.AddScoped<ContextBuilder>();
            services.AddScoped<AssistantService>();
            
            services.AddScoped<ToolExecutor>();
            services.AddScoped<INovaToolRegistry, NovaToolRegistry>();
            services.AddSingleton(new ChatClient(
                model: "gpt-5.4-mini",
                apiKey: configuration["OpenAI:ApiKey"]));

            services.AddScoped<OpenAiPlanner>();
            services.AddScoped<IAssistantPlanner>(sp =>
                new RetryAssistantPlanner(sp.GetRequiredService<OpenAiPlanner>()));
            services.AddScoped<IAssistantResponseGenerator, OpenAiResponseGenerator>();
            
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