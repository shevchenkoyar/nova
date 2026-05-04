using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Modules.Memory.Application;
using Nova.Modules.Memory.Application.Adapters;
using Nova.Modules.Memory.Application.Tools;
using Nova.Common.Application.Memory;
using Nova.Common.Application.Tools;
using Nova.Modules.Memory.Infrastructure.Database;
using Nova.Modules.Memory.Infrastructure.Database.Repositories;
using OpenAI.Embeddings;

namespace Nova.Modules.Memory.Infrastructure;

public static class MemoryModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMemoryModule(IConfiguration configuration)
        {
            services.AddDbContext<MemoryDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("nova-db"),
                    npgsql => npgsql.UseVector());
            });

            services.AddSingleton(_ =>
                new EmbeddingClient(
                    model: configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small",
                    apiKey: configuration["OpenAI:ApiKey"]!));

            services.AddScoped<IMemoryEmbeddingProvider, OpenAiMemoryEmbeddingProvider>();
            services.AddScoped<IMemoryRepository, PgVectorMemoryRepository>();

            services.AddScoped<MemoryService>();
            services.AddScoped<IAssistantMemory, MemoryAssistantMemory>();

            services.AddScoped<INovaTool, SaveFactTool>();
            services.AddScoped<INovaTool, SearchMemoryTool>();
            return services;
        }
    }
}