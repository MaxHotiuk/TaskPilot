using Application.Commands.Meetings;
using Domain.Dtos.Meetings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Meetings;

public class UpdateMeetingEndpoint : EndpointBaseWithRequest<UpdateMeetingCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/meetings/{id:guid}", async (
                Guid id,
                string title,
                string description,
                DateTime scheduledAt,
                int duration,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateMeetingCommand(id, title, description, scheduledAt, duration);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateMeeting")
            .WithTags("Meetings")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateMeetingCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
