using Microsoft.Extensions.DependencyInjection;
using Nava.Modules.Memory.Application.Tools;
using Nava.Modules.Memory.Contracts;
using Nova.Common.Application.Tools;

namespace Nava.Modules.Memory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMemoryModule(this IServiceCollection services)
    {
        services.AddSingleton<IMemoryRepository, InMemoryMemoryRepository>();
        services.AddSingleton<IMemoryModuleApi, MemoryModuleApi>();
        services.AddScoped<INovaTool, SaveFactTool>();
        services.AddScoped<INovaTool, SearchMemoryTool>();
        
        return services;
    }
}