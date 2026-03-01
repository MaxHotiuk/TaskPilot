using Application.Commands.OrganizationMembers;
using Application.Abstractions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.OrganizationMembers;

public class DemoteFromManagerEndpoint : EndpointBaseWithRequest<DemoteFromManagerCommand, Unit>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/organizations/{organizationId:guid}/members/{userId:guid}/demote", async (
                Guid organizationId,
                Guid userId,
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var demoterId = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(demoterId))
                    return Results.Unauthorized();

                var command = new DemoteFromManagerCommand(organizationId, userId, Guid.Parse(demoterId));
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("DemoteFromManager")
            .WithTags("Organizations")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(DemoteFromManagerCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
