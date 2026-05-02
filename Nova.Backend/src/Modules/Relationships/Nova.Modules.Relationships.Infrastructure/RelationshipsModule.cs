using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Common.Application.Tools;
using Nova.Modules.Relationships.Application;
using Nova.Modules.Relationships.Application.Tools;
using Nova.Modules.Relationships.Contracts;
using Nova.Modules.Relationships.Infrastructure.Database;
using Nova.Modules.Relationships.Infrastructure.Database.Repositories;

namespace Nova.Modules.Relationships.Infrastructure;

public static class RelationshipsModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRelationshipsModule(IConfiguration configuration)
        {
            services.AddScoped<IRelationshipsModuleApi, RelationshipsModuleApi>();
            
            services.AddScoped<INovaTool, GetRelationshipStatusTool>();
            
            var connectionString = configuration.GetConnectionString("nova-db");

            services.AddDbContext<RelationshipsDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IRelationshipProfileRepository, RelationshipProfileRepository>();
            
            return services;
        }
    }
}