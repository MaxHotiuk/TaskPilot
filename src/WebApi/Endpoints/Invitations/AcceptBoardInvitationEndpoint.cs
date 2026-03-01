using Application.Commands.Invitations;
using Application.Abstractions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Invitations;

public class AcceptBoardInvitationEndpoint : EndpointBaseWithRequest<AcceptBoardInvitationCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/invitations/boards/{invitationId:guid}/accept", async (
                Guid invitationId,
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var userIdString = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(userIdString))
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);
                var command = new AcceptBoardInvitationCommand(invitationId, userId);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("AcceptBoardInvitation")
            .WithTags("Invitations")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(AcceptBoardInvitationCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
