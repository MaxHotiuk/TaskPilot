using Application.Commands.Invitations;
using Application.Abstractions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Invitations;

public class AcceptOrganizationInvitationEndpoint : EndpointBaseWithRequest<AcceptOrganizationInvitationCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/invitations/organizations/{invitationId:guid}/accept", async (
                Guid invitationId,
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var userIdString = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(userIdString))
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);
                var command = new AcceptOrganizationInvitationCommand(invitationId, userId);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("AcceptOrganizationInvitation")
            .WithTags("Invitations")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(AcceptOrganizationInvitationCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
