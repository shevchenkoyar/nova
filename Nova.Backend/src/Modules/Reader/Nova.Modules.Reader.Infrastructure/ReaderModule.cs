using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nova.Common.Application.Tools;
using Nova.Modules.Reader.Application.Summarization;
using Nova.Modules.Reader.Application.Tools;
using Nova.Modules.Reader.Contracts;
using Nova.Modules.Reader.Infrastructure.Html;

namespace Nova.Modules.Reader.Infrastructure;

public static class ReaderModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddReaderModule(IConfiguration configuration)
        {
            services.Configure<ReaderInfrastructureOptions>(
                configuration.GetSection(ReaderInfrastructureOptions.SectionName));

            services.AddHttpClient<IPageReader, HtmlPageReader>((sp, client) =>
            {
                var options = sp
                    .GetRequiredService<IOptions<ReaderInfrastructureOptions>>()
                    .Value;
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });
            
            services.AddScoped<INovaTool, FetchUrlTool>();
            
            services.AddScoped<IPageSummarizer, OpenAiPageSummarizer>();
            services.AddScoped<INovaTool, SummarizePageTool>();
            return services;
        }
    }
}