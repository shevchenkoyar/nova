using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Application.Tools;
using Nova.Modules.Search.Application;
using Nova.Modules.Search.Contracts;

namespace Nova.Modules.Search.Infrastructure;

public static class SearchModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSearchModule()
        {
            services.AddScoped<INovaTool, SearchWebTool>();
            services.AddSingleton<ISearchProvider, FakeSearchProvider>();

            return services;
        }
    }
}