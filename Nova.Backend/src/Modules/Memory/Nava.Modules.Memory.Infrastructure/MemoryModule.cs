using Microsoft.Extensions.DependencyInjection;
using Nava.Modules.Memory.Application;
using Nava.Modules.Memory.Application.Adapters;
using Nava.Modules.Memory.Application.Tools;
using Nava.Modules.Memory.Contracts;
using Nova.Common.Application.Memory;
using Nova.Common.Application.Tools;

namespace Nava.Modules.Memory.Infrastructure;

public static class MemoryModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMemoryModule()
        {
            services.AddSingleton<IMemoryRepository, InMemoryMemoryRepository>();
            services.AddScoped<IMemoryModuleApi, MemoryModuleApi>();
            services.AddScoped<IAssistantMemory, MemoryAssistantMemory>();
            
            services.AddScoped<INovaTool, SaveFactTool>();
            services.AddScoped<INovaTool, SearchMemoryTool>();
            
            return services;
        }
    }
}