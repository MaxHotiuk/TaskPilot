using Application.Commands.Users;
using Domain.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Users;

public class UpdateUserEndpoint : EndpointBaseWithRequest<UpdateUserCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{id:guid}", async (
                Guid id,
                UpdateUserRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateUserCommand(
                    id,
                    dto.Email,
                    dto.Username,
                    dto.Role
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateUser")
            .WithTags("Users")
            .RequireAuthorization(Policies.RequireSelfUpdate)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateUserCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record UpdateUserRequestDto(string Email, string Username, string Role);
