using Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Users;

public class GetCurrentUserEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/me", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCurrentUserQuery();
                var result = await mediator.Send(query, cancellationToken);
                return HandleResult(result);
            })
            .WithName("GetCurrentUser")
            .WithTags("Users")
            .RequireAuthorization() // Require authentication
            .Produces<Application.Common.Dtos.Users.UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
