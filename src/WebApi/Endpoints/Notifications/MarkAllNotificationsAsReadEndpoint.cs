using Application.Commands.Notifications;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Notifications;

public class MarkAllNotificationsAsReadEndpoint : EndpointBaseWithRequest<MarkAllNotificationsAsReadCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notifications/mark-all-read/{userId:guid}", async (
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new MarkAllNotificationsAsReadCommand(userId);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("MarkAllNotificationsAsRead")
            .WithTags("Notifications")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(MarkAllNotificationsAsReadCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
