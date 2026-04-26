using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nova.Common.Application.Clock;
using Nova.Common.Application.Idempotency;
using Nova.Common.Infrastructure.Clock;
using Nova.Common.Infrastructure.Idempotency;

namespace Nova.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RedisIdempotencyOptions>(configuration.GetSection("Idempotency"));
        
        services.AddScoped<IIdempotencyService, RedisIdempotencyService>();
        
        services.TryAddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}