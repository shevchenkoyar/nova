using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Application.Tools;
using Nova.Modules.Clock.Application.Tools;

namespace Nova.Modules.Clock.Infrastructure;

public static class ClockModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddClockModule()
        {
            services.AddScoped<INovaTool, SystemTimeTool>();

            return services;
        }
    }
}