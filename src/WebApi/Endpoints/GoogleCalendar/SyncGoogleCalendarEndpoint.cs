using Application.Commands.GoogleCalendar;
using Domain.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebApi.Endpoints;

namespace WebApi.Endpoints.GoogleCalendar;

public class SyncGoogleCalendarEndpoint : EndpointBaseWithRequest<SyncGoogleCalendarCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/{userId:guid}/google-calendar/sync", async (
                Guid userId,
                SyncGoogleCalendarRequest dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new SyncGoogleCalendarCommand(userId, dto.Month);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("SyncGoogleCalendar")
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
        SyncGoogleCalendarCommand request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

/// <summary>Request body for the sync endpoint.</summary>
public record SyncGoogleCalendarRequest(DateTime Month);
