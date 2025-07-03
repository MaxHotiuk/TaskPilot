using Application.Commands.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Users;

public class CreateUserEndpoint : EndpointBaseWithRequest<CreateUserCommand, Guid>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", async (
                CreateUserCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateUser")
            .WithTags("Users")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateUserCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/users/{result}", result);
    }
}
