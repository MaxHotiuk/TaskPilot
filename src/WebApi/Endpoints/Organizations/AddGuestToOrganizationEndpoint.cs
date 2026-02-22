using Application.Commands.Organizations;
using Application.Abstractions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Organizations;

public class AddGuestToOrganizationEndpoint : EndpointBaseWithRequest<AddGuestToOrganizationCommand, Unit>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/organizations/{organizationId:guid}/guests", async (
                Guid organizationId,
                AddGuestRequest request,
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var managerId = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(managerId))
                    return Results.Unauthorized();

                var command = new AddGuestToOrganizationCommand(organizationId, request.UserEmail, Guid.Parse(managerId));
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("AddGuestToOrganization")
            .WithTags("Organizations")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(AddGuestToOrganizationCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record AddGuestRequest(string UserEmail);
