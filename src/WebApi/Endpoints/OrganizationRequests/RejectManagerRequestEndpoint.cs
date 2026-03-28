using Application.Commands.OrganizationRequests;
using Application.Abstractions.Authentication;
using Domain.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.OrganizationRequests;

public class RejectManagerRequestEndpoint : EndpointBaseWithRequest<RejectManagerRequestCommand, Unit>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/organizations/manager-requests/{requestId:guid}/reject", async (
                Guid requestId,
                RejectRequestBody body,
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var reviewerId = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(reviewerId))
                    return Results.Unauthorized();

                var command = new RejectManagerRequestCommand(requestId, Guid.Parse(reviewerId), body.ReviewNotes);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("RejectManagerRequest")
            .WithTags("Organizations")
            .RequireAuthorization(Policies.RequireAdminRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(RejectManagerRequestCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record RejectRequestBody(string? ReviewNotes);
