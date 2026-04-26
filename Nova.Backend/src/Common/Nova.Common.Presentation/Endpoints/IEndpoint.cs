using Microsoft.AspNetCore.Routing;

namespace Nova.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
