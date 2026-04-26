using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi;

namespace Nova.Common.Presentation.Idempotency;

public static class IdempotencyEndpointExtensions
{
    public static RouteHandlerBuilder RequireIdempotencyKey(this RouteHandlerBuilder builder)
    {
        builder.AddOpenApiOperationTransformer((operation, _, _) =>
        {
            operation.Parameters ??= [];

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = IdempotencyHeaders.IdempotencyKey,
                In = ParameterLocation.Header,
                Required = true,
                Description = "Idempotency key (UUID). Required for safe retries.",
                Schema = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" }
            });

            return Task.CompletedTask;
        });

        return builder;
    }
}
