using Application.Abstractions.Authentication;
using Application.Commands.GoogleCalendar;
using Domain.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebApi.Endpoints;

namespace WebApi.Endpoints.GoogleCalendar;

/// <summary>
/// Receives the OAuth2 authorization code from the Blazor frontend (which captured
/// it from Google's redirect), resolves the current user from the JWT, and dispatches
/// <see cref="ConnectGoogleCalendarCommand"/> to exchange the code for tokens.
/// </summary>
public class ConnectGoogleCalendarEndpoint : EndpointBaseWithRequest<ConnectGoogleCalendarCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/google-calendar/connect", async (
                ConnectGoogleCalendarRequest dto,
                IAuthenticationService authenticationService,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var userIdString = await authenticationService.GetCurrentUserIdAsync();

                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new ConnectGoogleCalendarCommand(userId, dto.Code);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("ConnectGoogleCalendar")
            .WithTags("GoogleCalendar")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(
        ConnectGoogleCalendarCommand request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
