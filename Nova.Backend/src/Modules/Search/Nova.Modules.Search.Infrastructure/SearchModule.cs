using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nova.Common.Application.Tools;
using Nova.Modules.Search.Application.Tools;
using Nova.Modules.Search.Contracts;
using Nova.Modules.Search.Infrastructure.Brave;

namespace Nova.Modules.Search.Infrastructure;

public static class SearchModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSearchModule(IConfiguration configuration)
        {
            services.AddScoped<INovaTool, SearchWebTool>();
            
            var provider = configuration.GetValue<string>("Search:Provider");
            
            if (string.Equals(provider, "Brave", StringComparison.OrdinalIgnoreCase))
            {
                services.UseBraveSearchProvider(configuration);
            }
            else
            {
                throw new InvalidOperationException($"Unknown search provider '{provider}'. Supported providers: Brave.");
            }

            return services;
        }
        
        private IServiceCollection UseBraveSearchProvider(IConfiguration configuration)
        {
            services.Configure<BraveSearchOptions>(configuration.GetSection(BraveSearchOptions.SectionName));
            
            services.AddHttpClient<ISearchProvider, BraveSearchProvider>((sp, client) =>
                {
                    var options = sp
                        .GetRequiredService<IOptions<BraveSearchOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(20);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression =
                        System.Net.DecompressionMethods.GZip |
                        System.Net.DecompressionMethods.Deflate |
                        System.Net.DecompressionMethods.Brotli
                });

            return services;
        }
    }
}