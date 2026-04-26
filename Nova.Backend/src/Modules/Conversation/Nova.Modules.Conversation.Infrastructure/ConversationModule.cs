using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Presentation.Endpoints;

namespace Nova.Modules.Conversation.Infrastructure;

public static class ConversationModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddConversationModule(IConfiguration configuration)
        {
            services.AddEndpoints(Presentation.AssemblyReference.Assembly);

            return services;
        }
    }
}