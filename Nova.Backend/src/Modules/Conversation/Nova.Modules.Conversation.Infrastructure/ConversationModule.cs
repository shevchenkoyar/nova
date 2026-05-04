using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Application.Assistant;
using Nova.Common.Application.Conversation;
using Nova.Common.Presentation.Endpoints;
using Nova.Modules.Conversation.Application;
using Nova.Modules.Conversation.Infrastructure.Database;
using Nova.Modules.Conversation.Infrastructure.Database.Repositories;

namespace Nova.Modules.Conversation.Infrastructure;

public static class ConversationModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddConversationModule(IConfiguration configuration)
        {
            services.AddScoped<ConversationHistoryService>();
            services.AddScoped<IConversationHistory, ConversationHistoryService>(sp => 
                sp.GetRequiredService<ConversationHistoryService>());
            
            services.AddEndpoints(Presentation.AssemblyReference.Assembly);
            
            var connectionString = configuration.GetConnectionString("nova-db");
            services.AddDbContext<ConversationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            services.AddScoped<IConversationMessageRepository, ConversationMessageRepository>();

            return services;
        }
    }
}