using Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Users;
using System.Collections.Generic;

namespace WebApi.Endpoints.Users;

public class GetAllUsersEndpoint : EndpointBaseWithRequest<GetAllUsersQuery, IEnumerable<UserDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetAllUsersQuery(), mediator, cancellationToken);
            })
            .WithName("GetAllUsers")
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetAllUsersQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
