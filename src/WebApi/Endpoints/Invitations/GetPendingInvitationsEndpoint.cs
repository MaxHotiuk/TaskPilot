using Application.Queries.Invitations;
using Application.Abstractions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Invitations;

public class GetPendingInvitationsEndpoint : EndpointBaseWithRequest<GetPendingInvitationsQuery, PendingInvitationsDto>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/invitations/pending", async (
                IMediator mediator,
                IAuthenticationService authService,
                CancellationToken cancellationToken) =>
            {
                var userIdString = await authService.GetCurrentUserIdAsync();
                if (string.IsNullOrEmpty(userIdString))
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);
                var query = new GetPendingInvitationsQuery(userId);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("GetPendingInvitations")
            .WithTags("Invitations")
            .RequireAuthorization()
            .Produces<PendingInvitationsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetPendingInvitationsQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
