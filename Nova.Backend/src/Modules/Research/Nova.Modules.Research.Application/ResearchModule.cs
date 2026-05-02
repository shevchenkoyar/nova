using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Application.Tools;
using Nova.Modules.Research.Application.Tools;
using Nova.Modules.Research.Contracts;

namespace Nova.Modules.Research.Application;

public static class ResearchModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddResearchModule(IConfiguration configuration)
        {
            services.AddScoped<IResearchService, ResearchService>();
            services.AddScoped<IResearchSynthesizer, OpenAiResearchSynthesizer>();
            services.AddScoped<INovaTool, RunResearchTool>();
            
            return services;
        }
    }
}