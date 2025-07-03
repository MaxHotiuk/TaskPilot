using Application.Commands.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Users;

public class UpdateUserRoleEndpoint : EndpointBaseWithRequest<UpdateUserRoleCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{userId:guid}/role", async (
                Guid userId,
                UpdateUserRoleRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateUserRoleCommand(userId, request.Role);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateUserRole")
            .WithTags("Users")
            .RequireAuthorization(Policies.RequireAdminRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateUserRoleCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record UpdateUserRoleRequest(string Role);
