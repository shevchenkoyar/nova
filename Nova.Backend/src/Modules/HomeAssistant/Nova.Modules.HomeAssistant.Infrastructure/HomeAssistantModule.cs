using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nova.Common.Application.Tools;
using Nova.Modules.HomeAssistant.Application;
using Nova.Modules.HomeAssistant.Application.Resolver;
using Nova.Modules.HomeAssistant.Application.Tools;
using Nova.Modules.HomeAssistant.Contracts;
using Nova.Modules.HomeAssistant.Infrastructure.Database;
using Nova.Modules.HomeAssistant.Infrastructure.Database.Repositories;

namespace Nova.Modules.HomeAssistant.Infrastructure;

public static class HomeAssistantModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddHomeAssistantModule(IConfiguration configuration)
        {
            services.AddScoped<HomeAssistantEntitySyncService>();
            services.AddScoped<IHomeAssistantCommandResolver, OpenAiHomeAssistantCommandResolver>();

            services.AddScoped<INovaTool, ControlHomeAssistantTool>();
            services.AddScoped<INovaTool, QueryHomeAssistantStateTool>();

            services.Configure<HomeAssistantOptions>(
                configuration.GetSection(HomeAssistantOptions.SectionName));
            
            services.AddHomeAssistantModuleDatabase(configuration);

            services.AddScoped<IHomeAssistantEntityRepository, HomeAssistantEntityRepository>();

            services.AddHostedService<HomeAssistantEntitySyncHostedService>();

            services.AddHttpClient<IHomeAssistantClient, HomeAssistantRestClient>((sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<HomeAssistantOptions>>()
                    .Value;

                if (string.IsNullOrWhiteSpace(options.BaseUrl))
                    throw new InvalidOperationException("HomeAssistant:BaseUrl is not configured.");

                if (string.IsNullOrWhiteSpace(options.AccessToken))
                    throw new InvalidOperationException("HomeAssistant:AccessToken is not configured.");

                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/'));
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
            
            return services;
        }
        
        public IServiceCollection AddHomeAssistantModuleDatabase(IConfiguration configuration)
        {
            services.AddDbContext<HomeAssistantDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("nova-db"));
            });
            
            return services;
        }
    }
}