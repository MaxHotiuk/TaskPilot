using Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Users;

namespace WebApi.Endpoints.Users;

public class GetUserByIdEndpoint : EndpointBaseWithRequest<GetUserByIdQuery, UserDto?>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetUserByIdQuery(id), mediator, cancellationToken);
            })
            .WithName("GetUserById")
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetUserByIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
