using Application.Abstractions.Calendar;
using Domain.Common.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebApi.Endpoints;

namespace WebApi.Endpoints.GoogleCalendar;

public class GetGoogleCalendarAuthUrlEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId:guid}/google-calendar/auth-url", (
                Guid userId,
                IGoogleCalendarService googleCalendarService) =>
            {
                var state = userId.ToString("N");
                var url = googleCalendarService.GetAuthorizationUrl(state);
                return Results.Ok(new { url });
            })
            .WithName("GetGoogleCalendarAuthUrl")
            .WithTags("GoogleCalendar")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
