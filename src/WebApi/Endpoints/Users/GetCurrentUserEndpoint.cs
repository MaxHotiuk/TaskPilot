using Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

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
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<Domain.Dtos.Users.CurrentUserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
